using NutriMind.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Domain.Interfaces.Repositories
{
    public interface IFoodLogRepository : IRepository<FoodLog>
    {
        Task<IEnumerable<FoodLog>> GetHistoryForUserAsync(
            Guid userId,
            DateTime startDate,
            DateTime endDate,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);

        Task<DailyIntakeSummary?> GetDailySummaryAsync(
            Guid userId,
            DateTime date,
            CancellationToken cancellationToken = default);

        // Unlike GetHistoryForUserAsync (which compares by raw .Date, intended for
        // internal operations that already know the exact UTC day of the log), this method
        // receives an already-computed UTC instant range (e.g. an Ecuador calendar day
        // converted to its equivalent UTC range) and compares directly, without truncating —
        // necessary so the "today"/"this week" queries requested by the client respect the
        // real calendar day in Ecuador rather than the server's UTC calendar day.
        Task<IEnumerable<FoodLog>> GetLogsInUtcRangeAsync(
            Guid userId,
            DateTime startUtcInclusive,
            DateTime endUtcExclusive,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
