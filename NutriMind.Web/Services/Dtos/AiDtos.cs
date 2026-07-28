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

// Matches NutriMind.API.Controllers.ChatResponse — api/Ai/chat now returns
// { "reply": "..." } instead of a bare JSON string (see AiApiService.ChatAsync).
public class ChatResponseDto
{
    [JsonPropertyName("reply")]
    public string Reply { get; set; } = string.Empty;
}
