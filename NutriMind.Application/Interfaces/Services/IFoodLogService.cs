using NutriMind.Application.Common;
using NutriMind.Application.DTOs.Foods;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
namespace NutriMind.Application.Interfaces.Services
{
    public interface IFoodLogService
    {
        Task<Result<FoodLogResponseDto>> CreateFoodLogAsync(Guid userId, CreateFoodLogDto dto, CancellationToken cancellationToken = default);
        Task<Result<FoodLogResponseDto>> UpdateFoodLogAsync(Guid userId, Guid foodLogId, UpdateFoodLogDto dto, CancellationToken cancellationToken = default);
        Task<Result<bool>> DeleteFoodLogAsync(Guid userId, Guid foodLogId, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<FoodLogResponseDto>>> GetFoodLogHistoryAsync(Guid userId, DateTime startDate, DateTime endDate, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Result<IEnumerable<FoodLogResponseDto>>> GetDailyConsumptionAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default);
        Task<Result<FoodLogResponseDto>> QuickAddFoodLogAsync(Guid userId, QuickAddFoodLogDto dto, CancellationToken cancellationToken = default);
        Task<Result<FoodLogResponseDto>> SmartAddFoodLogAsync(Guid userId, SmartAddFoodLogDto dto, CancellationToken cancellationToken = default);
    }
}