using System.Text.Json.Serialization;

namespace NutriMind.Mobile.Models.AI;

// Matches NutriMind.API.Controllers.ChatResponse — api/Ai/chat now returns
// an explicit { "reply": "..." } instead of a bare JSON string (see ApiService.SendChatMessageAsync).
public sealed class ChatResponseDto
{
    [JsonPropertyName("reply")]
    public string Reply { get; set; } = string.Empty;
}
