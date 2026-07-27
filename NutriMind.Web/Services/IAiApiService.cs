using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public interface IAiApiService
{
    Task<ApiResult<MealPlanResponseDto>> GenerateMealPlanAsync(AiMealPlanRequestDto request);
    Task<ApiResult<string>> ChatAsync(string message);
}
