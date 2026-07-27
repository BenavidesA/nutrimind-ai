#nullable enable
using System.Threading;
using System.Threading.Tasks;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.AI;
using NutriMind.Application.DTOs.MealPlans;
using NutriMind.Domain.Common;
namespace NutriMind.Application.Interfaces;
public interface IAiService
{
    Task<Result<CreateMealPlanDto>> GenerateMealPlanAsync(AiMealPlanRequestDto request, CancellationToken cancellationToken = default);
    Task<Result<string>> GenerateChatResponseAsync(string userMessage, CancellationToken cancellationToken = default);
    Task<Result<FoodEstimateDto>> EstimateFoodNutritionAsync(string foodName, CancellationToken cancellationToken = default);
}