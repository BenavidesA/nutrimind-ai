using Microsoft.EntityFrameworkCore;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces.Repositories;
using NutriMind.Infrastructure.Persistence.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Infrastructure.Persistence.Repositories
{
    public class FoodRepository : Repository<Food>, IFoodRepository
    {
        public FoodRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Food>> SearchFoodsAsync(
            string searchTerm,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var query = _context.Foods
                .AsNoTracking()
                .Where(f => string.IsNullOrEmpty(searchTerm) || f.Name.Contains(searchTerm));

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<Food?> GetByBarcodeAsync(
            string barcode,
            CancellationToken cancellationToken = default)
        {
            return await _context.Foods
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Barcode == barcode, cancellationToken);
        }
    }
}
