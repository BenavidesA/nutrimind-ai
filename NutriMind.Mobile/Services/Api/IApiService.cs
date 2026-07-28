using NutriMind.Mobile.Models.Auth;
using NutriMind.Mobile.Models.Dashboard;
using NutriMind.Mobile.Models.Food;
using NutriMind.Mobile.Models.MealPlans;
using NutriMind.Mobile.Models.Profile;

namespace NutriMind.Mobile.Services.Api;

public interface IApiService
{
    Task<LoginResponse?> LoginAsync(LoginRequest request);

    Task<string?> RegisterAsync(RegisterRequest request);

    Task<LoginResponse?> RefreshTokenAsync(string refreshToken);

    Task<bool> ForgotPasswordAsync(string email);
    Task<bool> VerifyResetCodeAsync(string email, string code);
    Task<bool> ResetPasswordAsync(string email, string code, string newPassword);
    Task<bool> ChangePasswordAsync(string currentPassword, string newPassword);

    // Remember to create an equivalent DTO on mobile, or use dynamic types/basic classes for now
    Task<string> GenerateMealPlanAsync(object request);
    Task<string> SendChatMessageAsync(string message);

    Task<UserProgressDto?> GetUserProgressAsync();

    Task<List<FoodItemDto>> GetFoodsAsync();
    Task<bool> AddFoodLogAsync(QuickAddFoodLogDto dto);
    Task<bool> DeleteFoodAsync(Guid foodId);
    Task<bool> UpdateFoodLogAsync(Guid foodLogId, UpdateFoodLogDto dto);
    Task<DashboardStatsDto?> GetDashboardStatsAsync(DateTime startDate, DateTime endDate);
    Task<bool> AddSmartFoodLogAsync(SmartAddFoodLogDto dto);
    Task<List<RankingEntryDto>> GetRankingAsync(int top = 10);
    Task<List<MealPlanResponseDto>> GetMealPlansAsync();
    Task<bool> CreateMealPlanAsync(CreateMealPlanDto dto);
    Task<bool> UpdateMealPlanAsync(Guid id, UpdateMealPlanDto dto);
    Task<ProfileResponseDto?> GetProfileAsync();
    Task<bool> UpdateProfileAsync(UpdateProfileDto dto);
    Task<bool> DeleteAccountAsync();
    Task<bool> DeleteMealPlanAsync(Guid id);
    Task<List<BadgeResponseDto>> GetBadgesAsync();
    Task<List<FoodLogResponseDto>> GetFoodLogHistoryAsync(DateTime startDate, DateTime endDate);

}