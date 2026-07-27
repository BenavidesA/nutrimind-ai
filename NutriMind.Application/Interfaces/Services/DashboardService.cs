#nullable enable
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.Dashboard;
using NutriMind.Application.Interfaces;
using NutriMind.Domain.Common;
using NutriMind.Domain.Interfaces.Repositories;

namespace NutriMind.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IFoodLogRepository _foodLogRepository;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(
        IFoodLogRepository foodLogRepository,
        ILogger<DashboardService> logger)
    {
        _foodLogRepository = foodLogRepository;
        _logger = logger;
    }

    public async Task<Result<DashboardStatsDto>> GetUserStatsAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        try
        {
            // startDate/endDate llegan como días calendario en hora Ecuador (el cliente ya los
            // calcula así) — se convierten al rango de instantes UTC equivalente antes de
            // comparar contra LogDate, que se guarda en UTC crudo.
            var startUtc = EcuadorTimeHelper.EcuadorDayStartToUtc(startDate);
            var endUtcExclusive = EcuadorTimeHelper.EcuadorDayStartToUtc(endDate).AddDays(1);

            var logs = await _foodLogRepository.GetLogsInUtcRangeAsync(userId, startUtc, endUtcExclusive, 1, int.MaxValue, cancellationToken);

            var stats = new DashboardStatsDto();

            if (logs != null && logs.Any())
            {
                // Usamos las propiedades exactas de tu entidad FoodLog
                stats.TotalCaloriesConsumed = logs.Sum(l => l.Calories);
                stats.TotalProtein = logs.Sum(l => l.Protein);
                stats.TotalCarbs = logs.Sum(l => l.Carbs);
                stats.TotalFat = logs.Sum(l => l.Fat);

                // Agrupamos por día calendario en hora Ecuador (no por .Date crudo en UTC),
                // para que un registro de la noche no aparezca agrupado en el día siguiente.
                stats.DailyBreakdown = logs.GroupBy(l => EcuadorTimeHelper.ToLocal(l.LogDate).Date)
                    .Select(g => new DailyStatDto
                    {
                        Date = g.Key.ToString("yyyy-MM-dd"),
                        Calories = g.Sum(l => l.Calories)
                    })
                    .OrderBy(d => d.Date)
                    .ToList();
            }

            return Result<DashboardStatsDto>.Success(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculando estadísticas para el usuario {UserId}", userId);
            return Result<DashboardStatsDto>.Failure("Ocurrió un error al calcular el dashboard.");
        }
    }
}