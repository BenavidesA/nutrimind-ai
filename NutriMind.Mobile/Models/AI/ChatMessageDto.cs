namespace NutriMind.Mobile.Models.AI;

public sealed class ChatMessageDto
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Role { get; init; } = string.Empty;  // "user" o "assistant"
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.Now;

    // Helpers para la vista XAML
    public bool IsUser => Role == "user";
    public bool IsAssistant => Role == "assistant";
}