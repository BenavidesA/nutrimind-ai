#nullable enable
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.AI;
using NutriMind.Application.DTOs.MealPlans;
using NutriMind.Application.Interfaces;
using NutriMind.Application.Settings;
using NutriMind.Domain.Common;
using NutriMind.Domain.Interfaces.Repositories;
using System.Linq;

namespace NutriMind.Application.Services;

public class GeminiAiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly GeminiSettings _settings;
    private readonly ILogger<GeminiAiService> _logger;
    private readonly IFoodRepository _foodRepository;

    public GeminiAiService(
        HttpClient httpClient,
        IOptions<GeminiSettings> settings,
        ILogger<GeminiAiService> logger,
        IFoodRepository foodRepository)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
        _logger = logger;
        _foodRepository = foodRepository;
    }

    public async Task<Result<CreateMealPlanDto>> GenerateMealPlanAsync(AiMealPlanRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(_settings.ApiKey))
                return Result<CreateMealPlanDto>.Failure("La API Key de Gemini no está configurada.");

            var alergias = request.Allergies.Count > 0 ? string.Join(", ", request.Allergies) : "Ninguna";
            var ahoraLocal = EcuadorTimeHelper.ToLocal(DateTime.UtcNow);

            // Catálogo real de alimentos: sin esto, la IA inventa FoodId que no existen en la BD
            // y el plan falla al guardarse (violación de foreign key).
            var foods = await _foodRepository.GetAllAsync(cancellationToken);
            var catalogo = string.Join("\n", foods.Select(f => $"- {f.Id}: {f.Name}"));

            if (string.IsNullOrWhiteSpace(catalogo))
                return Result<CreateMealPlanDto>.Failure("No hay alimentos en el catálogo para generar un plan.");

            // 1. Armamos un prompt agresivo y claro
            var prompt = $@"
Eres un nutricionista experto. Genera un plan de alimentación de {request.Days} días.
Calorías diarias: {request.TargetCalories}. Dieta: {request.DietType}. Alergias: {alergias}.

Catálogo de alimentos disponibles (usa EXCLUSIVAMENTE estos IDs para ""FoodId"", nunca inventes un ID):
{catalogo}

Devuelve ESTRICTAMENTE un JSON válido que coincida con esta estructura de C#. No incluyas texto extra, solo el JSON:
{{
  ""Name"": ""Plan {request.DietType} generado por IA"",
  ""StartDate"": ""{ahoraLocal:yyyy-MM-ddTHH:mm:ss}"",
  ""EndDate"": ""{ahoraLocal.AddDays(request.Days):yyyy-MM-ddTHH:mm:ss}"",
  ""TotalCaloriesPerDay"": {request.TargetCalories},
  ""PlannedMeals"": [
    {{
      ""Day"": ""{ahoraLocal:yyyy-MM-ddTHH:mm:ss}"",
      ""QuantityG"": 200,
      ""MealTypeId"": 1,
      ""FoodId"": ""(un Id real del catálogo de arriba)""
    }}
  ]
}}";

            // 2. Construimos el cuerpo para la API de Gemini (Usando JSON Mode nativo)
            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new { responseMimeType = "application/json" }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

       
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite:generateContent?key={_settings.ApiKey}";
            var response = await _httpClient.PostAsync(url, jsonContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                // Cambiamos el retorno para ver qué dice Google
                return Result<CreateMealPlanDto>.Failure($"Error de Google: {errorText}");
            }

            // 4. Parseamos la respuesta
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDocument = JsonDocument.Parse(responseString);

            // Navegamos por el árbol de respuesta de Gemini: candidates[0].content.parts[0].text
            var generatedText = jsonDocument.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString();

            if (string.IsNullOrEmpty(generatedText))
                return Result<CreateMealPlanDto>.Failure("La IA devolvió una respuesta vacía.");

            // 5. Deserializamos primero a un DTO intermedio con FoodId como string: Gemini a veces
            //    devuelve un FoodId mal formado en alguna comida puntual, y deserializar directo a
            //    Guid descarta el plan completo por un solo elemento inválido.
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var rawPlan = JsonSerializer.Deserialize<RawMealPlanDto>(generatedText, options);

            if (rawPlan == null)
                return Result<CreateMealPlanDto>.Failure("No se pudo estructurar el plan generado.");

            var validMeals = new List<CreatePlannedMealDto>();
            foreach (var rawMeal in rawPlan.PlannedMeals)
            {
                if (!Guid.TryParse(rawMeal.FoodId, out var foodId))
                {
                    _logger.LogWarning("Plan de IA: se descartó una comida con FoodId inválido: {FoodId}", rawMeal.FoodId);
                    continue;
                }

                validMeals.Add(new CreatePlannedMealDto
                {
                    Day = rawMeal.Day,
                    QuantityG = rawMeal.QuantityG,
                    MealTypeId = rawMeal.MealTypeId,
                    FoodId = foodId
                });
            }

            if (validMeals.Count == 0)
                return Result<CreateMealPlanDto>.Failure("La IA no devolvió ninguna comida válida para el plan. Intenta de nuevo.");

            var generatedPlan = new CreateMealPlanDto
            {
                Name = rawPlan.Name,
                StartDate = rawPlan.StartDate,
                EndDate = rawPlan.EndDate,
                TotalCaloriesPerDay = rawPlan.TotalCaloriesPerDay,
                PlannedMeals = validMeals
            };

            return Result<CreateMealPlanDto>.Success(generatedPlan);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico generando plan con IA");
            return Result<CreateMealPlanDto>.Failure("Ocurrió un error inesperado al procesar la IA.");
        }
    }

    // DTOs intermedios solo para parsear la respuesta cruda de Gemini de forma resiliente
    // (FoodId como string, no Guid) — nunca se exponen fuera de este servicio.
    private sealed class RawMealPlanDto
    {
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCaloriesPerDay { get; set; }
        public List<RawPlannedMealDto> PlannedMeals { get; set; } = new();
    }

    private sealed class RawPlannedMealDto
    {
        public DateTime Day { get; set; }
        public decimal QuantityG { get; set; }
        public int MealTypeId { get; set; }
        public string FoodId { get; set; } = string.Empty;
    }

    public async Task<Result<string>> GenerateChatResponseAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        try
        {
            // Prompt diseñado para ser nutricionista, NO para programador
            var chatPrompt = $@"Eres NutriMind AI, un nutricionista experto y empático. 
        Responde de forma natural, amigable y breve en texto plano. 
        NO uses JSON. Si el usuario te pregunta por alimentos, dales consejos saludables.";

            var requestBody = new
            {
                contents = new[] {
                new { parts = new[] { new { text = chatPrompt + " Usuario: " + userMessage } } }
            }
                // NOTA: Eliminamos 'generationConfig' de JSON para que responda libremente
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite:generateContent?key={_settings.ApiKey}";

            var response = await _httpClient.PostAsync(url, jsonContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result<string>.Failure($"Error de Google: {errorText}");
            }

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            // Extraer texto plano
            using var jsonDocument = JsonDocument.Parse(responseString);
            var text = jsonDocument.RootElement.GetProperty("candidates")[0]
                        .GetProperty("content").GetProperty("parts")[0]
                        .GetProperty("text").GetString();

            return Result<string>.Success(text ?? "No pude procesar tu mensaje.");
        }
        catch (Exception ex)
        {
            return Result<string>.Failure("Error al procesar el chat: " + ex.Message);
        }
    }

    public async Task<Result<FoodEstimateDto>> EstimateFoodNutritionAsync(string foodName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(_settings.ApiKey))
                return Result<FoodEstimateDto>.Failure("La API Key de Gemini no está configurada.");

            var prompt = $@"Eres un experto en nutrición y bases de datos alimentarias (USDA, tablas de composición de alimentos).
Estima los valores nutricionales promedio por cada 100 gramos del siguiente alimento: ""{foodName}"".

Reglas obligatorias:
1. Los valores deben ser nutricionalmente coherentes y cumplir la fórmula calórica aproximada:
   CaloriesPer100g ≈ (CarbsPer100g × 4) + (ProteinPer100g × 4) + (FatPer100g × 9)
   No devuelvas valores que se aparten significativamente de esta relación.
2. Usa como referencia estos ejemplos reales (valores por 100g):
   - Manzana: ~52 kcal, 14g carbs, 0.3g proteína, 0.2g grasa.
   - Pechuga de pollo (cocida): ~165 kcal, 0g carbs, 31g proteína, 3.6g grasa.
   - Arroz blanco (cocido): ~130 kcal, 28g carbs, 2.7g proteína, 0.3g grasa.
3. Si el nombre del alimento es ambiguo o genérico (ej. ""carne"" sin especificar tipo), asume la variante más común y representativa (ej. carne de res, corte magro) en vez de inventar valores arbitrarios.

Devuelve ESTRICTAMENTE un JSON válido con esta estructura exacta, sin texto adicional ni explicaciones:
{{
  ""CaloriesPer100g"": 0,
  ""ProteinPer100g"": 0,
  ""CarbsPer100g"": 0,
  ""FatPer100g"": 0
}}";

            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new { responseMimeType = "application/json" }
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-3.1-flash-lite:generateContent?key={_settings.ApiKey}";
            var response = await _httpClient.PostAsync(url, jsonContent, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorText = await response.Content.ReadAsStringAsync(cancellationToken);
                return Result<FoodEstimateDto>.Failure($"Error de Google: {errorText}");
            }

            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            using var jsonDocument = JsonDocument.Parse(responseString);
            var generatedText = jsonDocument.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text").GetString();

            if (string.IsNullOrEmpty(generatedText))
                return Result<FoodEstimateDto>.Failure("La IA devolvió una respuesta vacía.");

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var estimate = JsonSerializer.Deserialize<FoodEstimateDto>(generatedText, options);

            if (estimate == null)
                return Result<FoodEstimateDto>.Failure("No se pudo estructurar la estimación nutricional.");

            return Result<FoodEstimateDto>.Success(estimate);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error crítico estimando nutrición con IA");
            return Result<FoodEstimateDto>.Failure("Ocurrió un error inesperado al estimar la nutrición.");
        }
    }
}