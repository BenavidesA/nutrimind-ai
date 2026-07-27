using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public interface IMealPlanApiService
{
    Task<ApiResult<List<MealPlanResponseDto>>> GetAllAsync();
    Task<ApiResult<MealPlanResponseDto>> GetByIdAsync(Guid id);
    Task<ApiResult<MealPlanResponseDto>> CreateAsync(CreateMealPlanDto request);
    Task<ApiResult<MealPlanResponseDto>> UpdateAsync(Guid id, UpdateMealPlanDto request);
    Task<ApiResult> DeleteAsync(Guid id);
}
