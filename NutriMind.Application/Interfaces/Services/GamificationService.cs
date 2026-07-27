using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NutriMind.Application.Common; // Ajusta según el namespace de tu Result<T>
using NutriMind.Application.Interfaces;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces;
using NutriMind.Domain.Interfaces.Repositories;

namespace NutriMind.Application.Services
{
    public class GamificationService : IGamificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GamificationService> _logger;

        public GamificationService(IUnitOfWork unitOfWork, ILogger<GamificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<bool>> ProcessActivityAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var userRepo = _unitOfWork.Repository<User>();
                var user = await userRepo.GetByIdAsync(userId, cancellationToken);

                if (user == null)
                    return Result<bool>.Failure("Usuario no encontrado.");

                // Usamos .Date para ignorar las horas y minutos, solo nos importa el día — pero
                // convertido a hora Ecuador primero: LastLogDate se guarda en UTC crudo, y
                // comparar por .Date en UTC crudo rompe cerca de la medianoche Ecuador (UTC va
                // 5 horas adelante, así que entre ~7pm y medianoche Ecuador el servidor ya
                // "cree" que es el día siguiente).
                var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
                var lastLog = user.LastLogDate.HasValue
                    ? EcuadorTimeHelper.ToLocal(user.LastLogDate.Value).Date
                    : (DateTime?)null;

                if (lastLog == today)
                {
                    // Ya registró algo hoy, la racha se mantiene intacta.
                    // Igual verificamos medallas: la racha pudo haber calificado antes de que
                    // existieran medallas que otorgar (ej. seed de Badges corrido después).
                    await CheckAndAwardBadgesAsync(userId, cancellationToken);
                    return Result<bool>.Success(true);
                }

                if (lastLog == today.AddDays(-1))
                {
                    // Entró ayer y entró hoy: ¡La racha crece!
                    user.CurrentStreak++;
                    if (user.CurrentStreak > user.HighestStreak)
                        user.HighestStreak = user.CurrentStreak;
                }
                else
                {
                    // Pasó más de un día (racha rota) o es su primera vez
                    user.CurrentStreak = 1;
                    if (user.HighestStreak == 0) user.HighestStreak = 1;
                }

                // Recompensa estándar: 10 puntos por registrar actividad diaria
                user.TotalPoints += 10;
                user.LastLogDate = DateTime.UtcNow;

                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Después de procesar la racha, verificamos si ganó medallas
                await CheckAndAwardBadgesAsync(userId, cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // La gamificación es un efecto secundario del registro de comida, no el dato principal.
                // Si falla aquí, el food log ya se guardó con éxito antes de llamar a este método —
                // no queremos que un 500 en la racha/puntos haga pensar al usuario que perdió su registro.
                _logger.LogError(ex, "Error procesando actividad de gamificación para el usuario {UserId}", userId);
                return Result<bool>.Failure("Ocurrió un error al procesar la gamificación.");
            }
        }

        public async Task<Result<bool>> CheckAndAwardBadgesAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var userRepo = _unitOfWork.Repository<User>();
            var user = await userRepo.GetByIdAsync(userId, cancellationToken);

            if (user == null)
                return Result<bool>.Failure("Usuario no encontrado.");

            var badgeRepo = _unitOfWork.Repository<Badge>();
            var userBadgeRepo = _unitOfWork.Repository<UserBadge>();

            // Definimos las metas
            var metas = new Dictionary<int, string>
    {
        { 3, "Racha de 3 Días" },
        { 7, "Racha de 7 Días" }
    };

            foreach (var meta in metas)
            {
                // Si el usuario alcanzó la meta (ej. 3 días)
                if (user.CurrentStreak >= meta.Key)
                {
                    // Buscamos si la medalla existe en el catálogo
                    var badges = await badgeRepo.FindAsync(b => b.Name == meta.Value, cancellationToken);
                    var badge = badges.FirstOrDefault();

                    if (badge != null)
                    {
                        // Verificamos si el usuario ya la tiene para no dársela dos veces
                        var hasBadge = await userBadgeRepo.FindAsync(ub => ub.UserId == userId && ub.BadgeId == badge.Id, cancellationToken);

                        if (!hasBadge.Any())
                        {
                            // ¡Felicidades! Le otorgamos la medalla
                            var newAward = new UserBadge
                            {
                                UserId = userId,
                                BadgeId = badge.Id,
                                AwardedAt = DateTime.UtcNow
                            };
                            await userBadgeRepo.AddAsync(newAward, cancellationToken);
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<bool>.Success(true);
        }
    }
}