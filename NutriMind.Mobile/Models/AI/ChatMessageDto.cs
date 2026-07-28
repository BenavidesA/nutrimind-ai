namespace NutriMind.Mobile.Models.AI;

public sealed class ChatMessageDto
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Role { get; init; } = string.Empty;  // "user" or "assistant"
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.Now;

    // Helpers for the XAML view
    public bool IsUser => Role == "user";
    public bool IsAssistant => Role == "assistant";
}