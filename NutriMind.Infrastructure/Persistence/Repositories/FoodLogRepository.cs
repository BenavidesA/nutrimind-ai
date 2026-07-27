using Microsoft.EntityFrameworkCore;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces.Repositories;
using NutriMind.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Infrastructure.Persistence.Repositories
{
    public class FoodLogRepository : Repository<FoodLog>, IFoodLogRepository
    {
        public FoodLogRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<FoodLog>> GetHistoryForUserAsync(
            Guid userId,
            DateTime startDate,
            DateTime endDate,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await _context.FoodLogs
                .AsNoTracking()
                .Where(fl => fl.UserId == userId && fl.LogDate.Date >= startDate.Date && fl.LogDate.Date <= endDate.Date)
                .OrderByDescending(fl => fl.LogDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(fl => fl.Food)
                .Include(fl => fl.MealType)
                .ToListAsync(cancellationToken);
        }

        public async Task<DailyIntakeSummary?> GetDailySummaryAsync(
            Guid userId,
            DateTime date,
            CancellationToken cancellationToken = default)
        {
            return await _context.DailyIntakeSummaries
                .FirstOrDefaultAsync(
                    ds => ds.UserId == userId && ds.SummaryDate.Date == date.Date,
                    cancellationToken);
        }

        public async Task<IEnumerable<FoodLog>> GetLogsInUtcRangeAsync(
            Guid userId,
            DateTime startUtcInclusive,
            DateTime endUtcExclusive,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            return await _context.FoodLogs
                .AsNoTracking()
                .Where(fl => fl.UserId == userId && fl.LogDate >= startUtcInclusive && fl.LogDate < endUtcExclusive)
                .OrderByDescending(fl => fl.LogDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(fl => fl.Food)
                .Include(fl => fl.MealType)
                .ToListAsync(cancellationToken);
        }
    }
}
