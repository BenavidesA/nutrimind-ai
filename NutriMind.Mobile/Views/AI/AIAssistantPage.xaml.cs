using NutriMind.Mobile.ViewModels.AI;

namespace NutriMind.Mobile.Views.AI;

public partial class AIAssistantPage : ContentPage
{
    private readonly AIAssistantViewModel _viewModel;

    // Constructor sin parámetros (Shell lo instancia así al ser <ShellContent>)
    public AIAssistantPage()
    {
        InitializeComponent();
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(AIAssistantViewModel)) as AIAssistantViewModel)!;
        BindingContext = _viewModel;
    }

    // Constructor con inyección de dependencias (por si se navega directo, no solo vía Shell)
    public AIAssistantPage(AIAssistantViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.Messages.Clear();
    }
}