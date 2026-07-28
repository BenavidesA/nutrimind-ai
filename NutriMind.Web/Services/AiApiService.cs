using System.Net.Http.Json;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public class AiApiService : IAiApiService
{
    private readonly HttpClient _httpClient;

    public AiApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResult<MealPlanResponseDto>> GenerateMealPlanAsync(AiMealPlanRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/Ai/generate-meal-plan", request);
        return await ApiResponseReader.ReadAsync<MealPlanResponseDto>(response, "La IA no pudo generar el plan de alimentación.");
    }

    // AiController.Chat (API) explicitly returns { "reply": "..." } (it used to return a bare
    // string that fell through to StringOutputFormatter and came out as unquoted text/plain,
    // breaking any ReadFromJsonAsync). Hardened with its own try/catch: if the body is ever not
    // valid JSON (contract change, infrastructure error before the controller, etc.), we don't
    // let the exception propagate unhandled all the way to the browser.
    public async Task<ApiResult<string>> ChatAsync(string message)
    {
        HttpResponseMessage response;
        try
        {
            response = await _httpClient.PostAsJsonAsync("api/Ai/chat", new ChatRequestDto { Message = message });
        }
        catch (Exception ex)
        {
            return ApiResult<string>.Failure("No se pudo conectar con el asistente: " + ex.Message);
        }

        if (response.IsSuccessStatusCode)
        {
            try
            {
                var payload = await response.Content.ReadFromJsonAsync<ChatResponseDto>();
                return payload != null
                    ? ApiResult<string>.Success(payload.Reply)
                    : ApiResult<string>.Failure("El servidor devolvió una respuesta vacía.");
            }
            catch (Exception)
            {
                return ApiResult<string>.Failure("El asistente respondió en un formato inesperado.");
            }
        }

        var error = await response.Content.ReadAsStringAsync();
        return ApiResult<string>.Failure(string.IsNullOrWhiteSpace(error) ? "El asistente no respondió correctamente." : error);
    }
}
