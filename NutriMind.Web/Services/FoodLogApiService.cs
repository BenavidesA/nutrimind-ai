using System.Net.Http.Json;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public class FoodLogApiService : IFoodLogApiService
{
    private readonly HttpClient _httpClient;

    public FoodLogApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<List<FoodLogResponseDto>>> GetDailyLogAsync(DateTime date)
    {
        var response = await _httpClient.GetAsync($"api/FoodLogs?date={date:yyyy-MM-dd}");
        return await ApiResponseReader.ReadAsync<List<FoodLogResponseDto>>(response, "No se pudo cargar el registro del día.");
    }

    public async Task<ApiResult<FoodLogResponseDto>> SmartAddAsync(SmartAddFoodLogDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/FoodLogs/smart-add", request);
        return await ApiResponseReader.ReadAsync<FoodLogResponseDto>(response, "No se pudo agregar el alimento.");
    }

    public async Task<ApiResult<FoodLogResponseDto>> UpdateAsync(Guid id, UpdateFoodLogDto request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/FoodLogs/{id}", request);
        return await ApiResponseReader.ReadAsync<FoodLogResponseDto>(response, "No se pudo actualizar el registro.");
    }

    public async Task<ApiResult> DeleteAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/FoodLogs/{id}");
        return await ApiResponseReader.ReadAsync(response, "No se pudo eliminar el registro.");
    }
}
