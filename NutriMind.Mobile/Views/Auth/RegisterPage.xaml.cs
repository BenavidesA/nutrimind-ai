using NutriMind.Mobile.ViewModels.Auth;

namespace NutriMind.Mobile.Views.Auth;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;

    // Constructor vacío para respaldo del sistema de enrutamiento
    public RegisterPage()
    {
        InitializeComponent();
        _viewModel = Handler?.MauiContext?.Services.GetService<RegisterViewModel>()!;
        BindingContext = _viewModel;
    }

    // Constructor principal con inyección de dependencias
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}