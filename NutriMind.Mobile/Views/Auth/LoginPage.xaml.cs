using NutriMind.Mobile.ViewModels.Auth;

namespace NutriMind.Mobile.Views.Auth;

public partial class LoginPage : ContentPage
{
    private readonly LoginViewModel _viewModel;

    // Constructor sin parámetros (exigido por Shell/Routing: AppShell.xaml instancia esta
    // página vía {DataTemplate auth:LoginPage}, que requiere un constructor vacío).
    public LoginPage()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(LoginViewModel)) as LoginViewModel)!;

        BindingContext = _viewModel;
    }

    // Constructor con inyección de dependencias directa
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}