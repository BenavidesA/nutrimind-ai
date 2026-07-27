using NutriMind.Mobile.ViewModels.Home;

namespace NutriMind.Mobile.Views.Home;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage()
    {
        InitializeComponent();
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(HomeViewModel)) as HomeViewModel)!;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.LoadDataCommand.Execute(null);
    }
}