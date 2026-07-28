using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Models.AI;
using NutriMind.Mobile.Services.Api;

namespace NutriMind.Mobile.ViewModels.AI;

public partial class AIAssistantViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string _currentMessage = string.Empty;

    [ObservableProperty]
    private bool _isTyping;

    public ObservableCollection<ChatMessageDto> Messages { get; } = new();
    public ObservableCollection<string> SuggestionChips { get; } = new();

    // Inject the ApiService via the constructor
    public AIAssistantViewModel(IApiService apiService)
    {
        _apiService = apiService;
        LoadInitialState();
    }

    private void LoadInitialState()
    {
        SuggestionChips.Add("¿Qué desayuné?");
        SuggestionChips.Add("Dame una dieta");
        SuggestionChips.Add("¿Cómo van mis macros?");
    }

    [RelayCommand]
    private async Task SendMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(CurrentMessage)) return;

        var userMsg = CurrentMessage.Trim();
        CurrentMessage = string.Empty;

        // 1. Add the user's message to the UI
        Messages.Add(new ChatMessageDto { Role = "user", Content = userMsg });

        // 2. Turn on the "typing..." text
        IsTyping = true;

        try
        {
            string responseContent;

            if (userMsg.ToLower().Contains("dieta") || userMsg.ToLower().Contains("plan"))
            {
                var request = new { targetCalories = 2000, days = 1, allergies = new List<string> { "Mariscos" }, dietType = "Equilibrado" };
                responseContent = await _apiService.GenerateMealPlanAsync(request);
            }
            else
            {
                // Regular chat question
                responseContent = await _apiService.SendChatMessageAsync(userMsg);
            }

            // 3. Print the response
            Messages.Add(new ChatMessageDto { Role = "assistant", Content = responseContent ?? "Sin respuesta." });
        }
        catch (Exception ex)
        {
            // We show the real error message/code instead of a generic one,
            // so backend failures can be diagnosed (e.g. an AI plan that fails to save).
            Messages.Add(new ChatMessageDto { Role = "assistant", Content = $"Error: {ex.Message}" });
        }
        finally
        {
            // 4. THE FINALLY BLOCK: Turns off the "typing..." text regardless of success or failure
            MainThread.BeginInvokeOnMainThread(() =>
            {
                IsTyping = false;
            });
        }
    }

    [RelayCommand]
    private async Task SendSuggestionAsync(string suggestion)
    {
        CurrentMessage = suggestion;
        await SendMessageAsync();
    }

    [RelayCommand]
    private void NewConversation()
    {
        Messages.Clear();
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("//home");
    }


}