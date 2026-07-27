using System.Text.Json.Serialization;

namespace NutriMind.Mobile.Models.AI;

// Coincide con NutriMind.API.Controllers.ChatResponse — api/Ai/chat ahora devuelve
// { "reply": "..." } explícito en vez de un string JSON pelado (ver ApiService.SendChatMessageAsync).
public sealed class ChatResponseDto
{
    [JsonPropertyName("reply")]
    public string Reply { get; set; } = string.Empty;
}
