using System.Net.Http.Json;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public class MealPlanApiService : IMealPlanApiService
{
    private readonly HttpClient _httpClient;

    public MealPlanApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<List<MealPlanResponseDto>>> GetAllAsync()
    {
        var response = await _httpClient.GetAsync("api/MealPlans");
        return await ApiResponseReader.ReadAsync<List<MealPlanResponseDto>>(response, "No se pudieron cargar los planes de alimentación.");
    }

    public async Task<ApiResult<MealPlanResponseDto>> GetByIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"api/MealPlans/{id}");
        return await ApiResponseReader.ReadAsync<MealPlanResponseDto>(response, "No se encontró el plan de alimentación.");
    }

    public async Task<ApiResult<MealPlanResponseDto>> CreateAsync(CreateMealPlanDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/MealPlans", request);
        return await ApiResponseReader.ReadAsync<MealPlanResponseDto>(response, "No se pudo crear el plan de alimentación.");
    }

    public async Task<ApiResult<MealPlanResponseDto>> UpdateAsync(Guid id, UpdateMealPlanDto request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/MealPlans/{id}", request);
        return await ApiResponseReader.ReadAsync<MealPlanResponseDto>(response, "No se pudo actualizar el plan de alimentación.");
    }

    public async Task<ApiResult> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/MealPlans/{id}");
        return await ApiResponseReader.ReadAsync(response, "No se pudo eliminar el plan de alimentación.");
    }
}
