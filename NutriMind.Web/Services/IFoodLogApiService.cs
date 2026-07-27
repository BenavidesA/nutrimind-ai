using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public interface IFoodLogApiService
{
    Task<ApiResult<List<FoodLogResponseDto>>> GetDailyLogAsync(DateTime date);
    Task<ApiResult<FoodLogResponseDto>> SmartAddAsync(SmartAddFoodLogDto request);
    Task<ApiResult<FoodLogResponseDto>> UpdateAsync(Guid id, UpdateFoodLogDto request);
    Task<ApiResult> DeleteAsync(Guid id);
}
