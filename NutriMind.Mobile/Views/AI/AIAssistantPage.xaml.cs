using NutriMind.Mobile.ViewModels.AI;

namespace NutriMind.Mobile.Views.AI;

public partial class AIAssistantPage : ContentPage
{
    private readonly AIAssistantViewModel _viewModel;

    // Parameterless constructor (Shell instantiates it this way as a <ShellContent>)
    public AIAssistantPage()
    {
        InitializeComponent();
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(AIAssistantViewModel)) as AIAssistantViewModel)!;
        BindingContext = _viewModel;
    }

    // Constructor with dependency injection (in case of direct navigation, not just via Shell)
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