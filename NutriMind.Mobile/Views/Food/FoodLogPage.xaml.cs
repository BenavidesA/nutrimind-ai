using NutriMind.Mobile.ViewModels.Food;

namespace NutriMind.Mobile.Views.Food;

public partial class FoodLogPage : ContentPage
{
    private readonly FoodLogViewModel _viewModel;

    // Parameterless constructor (required by Shell / Routing)
    public FoodLogPage()
    {
        InitializeComponent();

        // Definitive fix for the Handler error: access it via Application.Current
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(FoodLogViewModel)) as FoodLogViewModel)!;

        BindingContext = _viewModel;
    }

    // Constructor with Dependency Injection (main)
    public FoodLogPage(FoodLogViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Force the list to reload from the database every time this screen appears
        _viewModel?.LoadFoodsCommand.Execute(null);
    }
}