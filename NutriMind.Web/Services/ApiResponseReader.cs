using System.Net.Http.Json;

namespace NutriMind.Web.Services;

// Misma lógica que los ReadResultAsync privados de AuthApiService, extraída para reusar en
// todos los *ApiService nuevos sin duplicarla en cada uno.
public static class ApiResponseReader
{
    public static async Task<ApiResult<T>> ReadAsync<T>(HttpResponseMessage response, string defaultError)
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

    public static async Task<ApiResult> ReadAsync(HttpResponseMessage response, string defaultError)
    {
        if (response.IsSuccessStatusCode)
            return ApiResult.Success();

        var error = await response.Content.ReadAsStringAsync();
        return ApiResult.Failure(string.IsNullOrWhiteSpace(error) ? defaultError : error);
    }
}
