using System.Text.Json.Serialization;

namespace NutriMind.Web.Services.Dtos;

public class AiMealPlanRequestDto
{
    public decimal TargetCalories { get; set; }
    public int Days { get; set; } = 1;
    public List<string> Allergies { get; set; } = new();
    public string DietType { get; set; } = "Cualquiera";
}

public class ChatRequestDto
{
    public string Message { get; set; } = string.Empty;
}

// Coincide con NutriMind.API.Controllers.ChatResponse — api/Ai/chat ahora devuelve
// { "reply": "..." } en vez de un string JSON pelado (ver AiApiService.ChatAsync).
public class ChatResponseDto
{
    [JsonPropertyName("reply")]
    public string Reply { get; set; } = string.Empty;
}
