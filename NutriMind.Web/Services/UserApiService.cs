using System.Net.Http.Json;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public class UserApiService : IUserApiService
{
    private readonly HttpClient _httpClient;

    public UserApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<ProgressDto>> GetProgressAsync()
    {
        var response = await _httpClient.GetAsync("api/User/progress");
        return await ApiResponseReader.ReadAsync<ProgressDto>(response, "No se pudo cargar tu progreso.");
    }

    public async Task<ApiResult<ProfileResponseDto>> GetProfileAsync()
    {
        var response = await _httpClient.GetAsync("api/User/profile");
        return await ApiResponseReader.ReadAsync<ProfileResponseDto>(response, "No se pudo cargar tu perfil.");
    }

    public async Task<ApiResult<ProfileResponseDto>> UpdateProfileAsync(UpdateProfileDto request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/User/profile", request);
        return await ApiResponseReader.ReadAsync<ProfileResponseDto>(response, "No se pudo actualizar tu perfil.");
    }

    public async Task<ApiResult> ChangePasswordAsync(ChangePasswordDto request)
    {
        var response = await _httpClient.PutAsJsonAsync("api/User/change-password", request);
        return await ApiResponseReader.ReadAsync(response, "No se pudo cambiar la contraseña.");
    }

    public async Task<ApiResult> DeleteAccountAsync()
    {
        var response = await _httpClient.DeleteAsync("api/User/account");
        return await ApiResponseReader.ReadAsync(response, "No se pudo eliminar la cuenta.");
    }

    public async Task<ApiResult<List<BadgeResponseDto>>> GetBadgesAsync()
    {
        var response = await _httpClient.GetAsync("api/User/badges");
        return await ApiResponseReader.ReadAsync<List<BadgeResponseDto>>(response, "No se pudieron cargar tus medallas.");
    }
}
