using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public class DashboardApiService : IDashboardApiService
{
    private readonly HttpClient _httpClient;

    public DashboardApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<DashboardStatsDto>> GetStatsAsync(DateTime startDate, DateTime endDate)
    {
        var response = await _httpClient.GetAsync($"api/Dashboard/stats?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
        return await ApiResponseReader.ReadAsync<DashboardStatsDto>(response, "No se pudieron cargar las estadísticas.");
    }
}
