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

        // A diferencia de GetHistoryForUserAsync (que compara por .Date crudo, pensado para
        // operaciones internas que ya conocen el día UTC exacto del registro), este método
        // recibe un rango de instantes UTC ya calculado (ej. un día calendario de Ecuador
        // convertido a su rango UTC equivalente) y compara directo, sin truncar — necesario
        // para que las consultas "de hoy"/"de esta semana" pedidas por el cliente respeten el
        // día calendario real en Ecuador y no el día calendario UTC del servidor.
        Task<IEnumerable<FoodLog>> GetLogsInUtcRangeAsync(
            Guid userId,
            DateTime startUtcInclusive,
            DateTime endUtcExclusive,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default);
    }
}
