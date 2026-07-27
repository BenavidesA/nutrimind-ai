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

    // AiController.Chat (API) devuelve { "reply": "..." } explícito (antes devolvía un string
    // pelado que caía en StringOutputFormatter y salía como text/plain sin comillas, rompiendo
    // cualquier ReadFromJsonAsync). Blindado con try/catch propio: si el body alguna vez no es
    // JSON válido (cambio de contrato, error de infraestructura antes del controller, etc.), no
    // dejamos que la excepción se propague sin manejar hasta el navegador.
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
