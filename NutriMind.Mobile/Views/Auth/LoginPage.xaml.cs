using NutriMind.Mobile.ViewModels.Auth;

namespace NutriMind.Mobile.Views.Auth;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    // Parameterless constructor (required by Shell/Routing: AppShell.xaml instantiates this
    // page via {DataTemplate auth:LoginPage}, which requires an empty constructor).
    public LoginPage()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(LoginViewModel)) as LoginViewModel)!;

        BindingContext = _viewModel;
    }

    // Constructor with direct dependency injection
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}