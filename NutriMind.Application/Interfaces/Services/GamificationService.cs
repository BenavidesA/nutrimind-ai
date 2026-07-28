using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NutriMind.Application.Common; // Adjust according to your Result<T>'s namespace
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

                // We use .Date to ignore hours and minutes, we only care about the day — but
                // converted to Ecuador time first: LastLogDate is stored in raw UTC, and
                // comparing by .Date in raw UTC breaks near midnight in Ecuador (UTC runs
                // 5 hours ahead, so between ~7pm and midnight Ecuador time the server already
                // "thinks" it's the next day).
                var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
                var lastLog = user.LastLogDate.HasValue
                    ? EcuadorTimeHelper.ToLocal(user.LastLogDate.Value).Date
                    : (DateTime?)null;

                if (lastLog == today)
                {
                    // Already logged something today, the streak stays intact.
                    // We still check badges: the streak could have qualified before
                    // any badges existed to award (e.g. Badges seed run afterward).
                    await CheckAndAwardBadgesAsync(userId, cancellationToken);
                    return Result<bool>.Success(true);
                }

                if (lastLog == today.AddDays(-1))
                {
                    // Logged in yesterday and logged in today: the streak grows!
                    user.CurrentStreak++;
                    if (user.CurrentStreak > user.HighestStreak)
                        user.HighestStreak = user.CurrentStreak;
                }
                else
                {
                    // More than a day passed (streak broken) or it's their first time
                    user.CurrentStreak = 1;
                    if (user.HighestStreak == 0) user.HighestStreak = 1;
                }

                // Standard reward: 10 points for logging daily activity
                user.TotalPoints += 10;
                user.LastLogDate = DateTime.UtcNow;

                userRepo.Update(user);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // After processing the streak, check whether badges were earned
                await CheckAndAwardBadgesAsync(userId, cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Gamification is a side effect of logging food, not the primary data.
                // If this fails, the food log has already been saved successfully before calling this method —
                // we don't want a 500 on the streak/points to make the user think they lost their log.
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

            // We define the goals
            var metas = new Dictionary<int, string>
    {
        { 3, "Racha de 3 Días" },
        { 7, "Racha de 7 Días" }
    };

            foreach (var meta in metas)
            {
                // If the user reached the goal (e.g. 3 days)
                if (user.CurrentStreak >= meta.Key)
                {
                    // We check whether the badge exists in the catalog
                    var badges = await badgeRepo.FindAsync(b => b.Name == meta.Value, cancellationToken);
                    var badge = badges.FirstOrDefault();

                    if (badge != null)
                    {
                        // We check whether the user already has it, so it isn't awarded twice
                        var hasBadge = await userBadgeRepo.FindAsync(ub => ub.UserId == userId && ub.BadgeId == badge.Id, cancellationToken);

                        if (!hasBadge.Any())
                        {
                            // Congratulations! We award the badge
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