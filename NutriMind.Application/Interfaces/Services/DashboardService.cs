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
            // startDate/endDate arrive as calendar days in Ecuador time (the client already
            // computes them that way) — they're converted to the equivalent UTC instant range
            // before comparing against LogDate, which is stored in raw UTC.
            var startUtc = EcuadorTimeHelper.EcuadorDayStartToUtc(startDate);
            var endUtcExclusive = EcuadorTimeHelper.EcuadorDayStartToUtc(endDate).AddDays(1);

            var logs = await _foodLogRepository.GetLogsInUtcRangeAsync(userId, startUtc, endUtcExclusive, 1, int.MaxValue, cancellationToken);

            var stats = new DashboardStatsDto();

            if (logs != null && logs.Any())
            {
                // We use the exact properties of the FoodLog entity
                stats.TotalCaloriesConsumed = logs.Sum(l => l.Calories);
                stats.TotalProtein = logs.Sum(l => l.Protein);
                stats.TotalCarbs = logs.Sum(l => l.Carbs);
                stats.TotalFat = logs.Sum(l => l.Fat);

                // We group by calendar day in Ecuador time (not by raw .Date in UTC),
                // so a nighttime log doesn't end up grouped into the next day.
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