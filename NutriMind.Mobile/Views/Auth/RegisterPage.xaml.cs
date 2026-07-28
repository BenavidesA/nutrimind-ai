using NutriMind.Mobile.ViewModels.Auth;

namespace NutriMind.Mobile.Views.Auth;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;

    // Parameterless constructor as a fallback for the routing system
    public RegisterPage()
    {
        InitializeComponent();
        _viewModel = Handler?.MauiContext?.Services.GetService<RegisterViewModel>()!;
        BindingContext = _viewModel;
    }

    // Main constructor with dependency injection
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}