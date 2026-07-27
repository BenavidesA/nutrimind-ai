using NutriMind.Domain.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Domain.Interfaces.Repositories
{
    public interface IFoodRepository : IRepository<Food>
    {
        Task<IEnumerable<Food>> SearchFoodsAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Food?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    }
}
