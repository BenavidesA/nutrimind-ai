using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public interface IDashboardApiService
{
    Task<ApiResult<DashboardStatsDto>> GetStatsAsync(DateTime startDate, DateTime endDate);
}
