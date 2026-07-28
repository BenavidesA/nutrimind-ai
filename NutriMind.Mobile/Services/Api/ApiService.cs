using System;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NutriMind.Mobile.Models.AI;
using NutriMind.Mobile.Models.Auth;
using NutriMind.Mobile.Models.Dashboard;
using NutriMind.Mobile.Models.Food;
using NutriMind.Mobile.Models.MealPlans;
using NutriMind.Mobile.Models.Profile;
using NutriMind.Mobile.Helpers;
using System.Linq;
namespace NutriMind.Mobile.Services.Api;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiService> _logger;

    public ApiService(HttpClient httpClient, ILogger<ApiService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task<string?> RegisterAsync(RegisterRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsStringAsync();

        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task<LoginResponse?> RefreshTokenAsync(string refreshToken)
    {
        var response = await _httpClient.PostAsJsonAsync(
            "api/auth/refresh-token",
            new { refreshToken });

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }

        return null;
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", new ForgotPasswordRequest { Email = email });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> VerifyResetCodeAsync(string email, string code)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/verify-reset-code", new VerifyResetCodeRequest { Email = email, Code = code });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ResetPasswordAsync(string email, string code, string newPassword)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password", new ResetPasswordRequest { Email = email, Code = code, NewPassword = newPassword });
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var response = await _httpClient.PutAsJsonAsync("api/user/change-password", new ChangePasswordRequest { CurrentPassword = currentPassword, NewPassword = newPassword });
        return response.IsSuccessStatusCode;
    }

    public async Task<string> GenerateMealPlanAsync(object mealPlanRequest)
    {
        var response = await _httpClient.PostAsJsonAsync("api/ai/generate-meal-plan", mealPlanRequest);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadAsStringAsync();
        }

        throw new Exception(await response.Content.ReadAsStringAsync());
    }

    public async Task<UserProgressDto?> GetUserProgressAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<UserProgressDto>("api/user/progress");
        return response;
    }


    public async Task<List<FoodItemDto>> GetFoodsAsync()
    {
        try
        {
            var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
            var response = await _httpClient.GetFromJsonAsync<List<FoodLogResponseDto>>(
                $"api/FoodLogs?date={today:yyyy-MM-dd}");

            if (response == null) return new List<FoodItemDto>();

            return response.Select(log => new FoodItemDto
            {
                Id = log.Id.ToString(),
                Name = log.FoodName,
                Calories = (double)log.Calories,
                Carbs = (double)log.Carbs,
                Protein = (double)log.Protein,
                Fat = (double)log.Fat,
                ServingSizeG = (double)log.QuantityG,
                MealTypeId = log.MealTypeId,
                LogDate = log.LogDate,
                Notes = log.Notes
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer FoodLog");
            return new List<FoodItemDto>();
        }
    }

    public async Task<bool> AddFoodLogAsync(QuickAddFoodLogDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/FoodLogs/quick-add", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteFoodAsync(Guid foodId)
    {
        var response = await _httpClient.DeleteAsync($"api/FoodLogs/{foodId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateFoodLogAsync(Guid foodLogId, UpdateFoodLogDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/FoodLogs/{foodLogId}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<string> SendChatMessageAsync(string message)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/ai/chat", new { message });

            if (response.IsSuccessStatusCode)
            {
                // The endpoint now returns explicit JSON { "reply": "..." } instead of a
                // bare string (previously ReadAsStringAsync returned the raw text as-is because
                // the ASP.NET Core formatter sent unquoted text/plain for a loose string;
                // that broke NutriMind.Web, which does expect JSON — see ChatResponseDto).
                var payload = await response.Content.ReadFromJsonAsync<ChatResponseDto>();
                return payload?.Reply ?? "Sin respuesta.";
            }

            return "El asistente no respondió correctamente.";
        }
        catch (Exception ex)
        {
            return $"Error de conexión al chat: {ex.Message}";
        }
    }

    public async Task<DashboardStatsDto?> GetDashboardStatsAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var url = $"api/Dashboard/stats?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<DashboardStatsDto>(url);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer Dashboard");
            return null;
        }
    }

    public async Task<bool> AddSmartFoodLogAsync(SmartAddFoodLogDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/FoodLogs/smart-add", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<RankingEntryDto>> GetRankingAsync(int top = 10)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<RankingEntryDto>>($"api/User/ranking?top={top}");
            return response ?? new List<RankingEntryDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer ranking");
            return new List<RankingEntryDto>();
        }
    }

    public async Task<List<MealPlanResponseDto>> GetMealPlansAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<MealPlanResponseDto>>("api/MealPlans");
            return response ?? new List<MealPlanResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer MealPlans");
            return new List<MealPlanResponseDto>();
        }
    }

    public async Task<bool> CreateMealPlanAsync(CreateMealPlanDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("api/MealPlans", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> UpdateMealPlanAsync(Guid id, UpdateMealPlanDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/MealPlans/{id}", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<ProfileResponseDto?> GetProfileAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ProfileResponseDto>("api/User/profile");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer el perfil");
            return null;
        }
    }

    public async Task<bool> UpdateProfileAsync(UpdateProfileDto dto)
    {
        var response = await _httpClient.PutAsJsonAsync("api/User/profile", dto);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAccountAsync()
    {
        var response = await _httpClient.DeleteAsync("api/User/account");

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync();
            _logger.LogWarning("Error al eliminar cuenta ({StatusCode}): {ErrorBody}", (int)response.StatusCode, errorBody);
        }

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteMealPlanAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/MealPlans/{id}");
        return response.IsSuccessStatusCode;
    }

    public async Task<List<BadgeResponseDto>> GetBadgesAsync()
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<BadgeResponseDto>>("api/User/badges");
            return response ?? new List<BadgeResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer medallas");
            return new List<BadgeResponseDto>();
        }
    }

    public async Task<List<FoodLogResponseDto>> GetFoodLogHistoryAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            var url = $"api/FoodLogs/history?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&page=1&pageSize=3000";
            var response = await _httpClient.GetFromJsonAsync<List<FoodLogResponseDto>>(url);
            return response ?? new List<FoodLogResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al leer el historial");
            return new List<FoodLogResponseDto>();
        }
    }
}