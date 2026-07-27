using NutriMind.Application.Common;
using NutriMind.Application.DTOs.Foods;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Application.Interfaces.Services
{
    public interface IFoodService
    {
        Task<Result<FoodDto?>> GetFoodByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<FoodDto>>> SearchFoodsAsync(string? searchTerm, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Result<FoodDto?>> GetFoodByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    }
}
