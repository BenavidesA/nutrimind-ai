using System.Net.Http.Headers;
using System.Net.Http.Json;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public class AuthApiService : IAuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<AuthResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);
        return await ReadResultAsync<AuthResponseDto>(response, "No se pudo iniciar sesión.");
    }

    public async Task<ApiResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
        return await ReadResultAsync<AuthResponseDto>(response, "No se pudo completar el registro.");
    }

    public async Task<ApiResult> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/forgot-password", request);
        return await ReadResultAsync(response, "No se pudo procesar la solicitud.");
    }

    public async Task<ApiResult> VerifyResetCodeAsync(VerifyResetCodeRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/verify-reset-code", request);
        return await ReadResultAsync(response, "El código es inválido o expiró.");
    }

    public async Task<ApiResult> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/auth/reset-password", request);
        return await ReadResultAsync(response, "No se pudo actualizar la contraseña.");
    }

    public async Task<ApiResult> LogoutAsync(string accessToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request);
        return await ReadResultAsync(response, "No se pudo cerrar la sesión en el servidor.");
    }

    private static async Task<ApiResult<T>> ReadResultAsync<T>(HttpResponseMessage response, string defaultError)
    {
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadFromJsonAsync<T>();
            return data != null
                ? ApiResult<T>.Success(data)
                : ApiResult<T>.Failure("El servidor devolvió una respuesta vacía.");
        }

        var error = await response.Content.ReadAsStringAsync();
        return ApiResult<T>.Failure(string.IsNullOrWhiteSpace(error) ? defaultError : error);
    }

    private static async Task<ApiResult> ReadResultAsync(HttpResponseMessage response, string defaultError)
    {
        if (response.IsSuccessStatusCode)
            return ApiResult.Success();

        var error = await response.Content.ReadAsStringAsync();
        return ApiResult.Failure(string.IsNullOrWhiteSpace(error) ? defaultError : error);
    }
}
