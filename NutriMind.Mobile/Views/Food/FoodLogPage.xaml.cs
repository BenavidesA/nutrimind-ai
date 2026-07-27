using NutriMind.Mobile.ViewModels.Food;

namespace NutriMind.Mobile.Views.Food;

public partial class FoodLogPage : ContentPage
{
    private readonly FoodLogViewModel _viewModel;

    // Constructor sin parámetros (Exigido por Shell / Routing)
    public FoodLogPage()
    {
        InitializeComponent();

        // Solución definitiva al error de Handler: Accedemos desde Application.Current
        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(FoodLogViewModel)) as FoodLogViewModel)!;

        BindingContext = _viewModel;
    }

    // Constructor con Inyección de Dependencias (Principal)
    public FoodLogPage(FoodLogViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Obligamos a recargar la lista desde la base de datos SIEMPRE que esta pantalla aparezca
        _viewModel?.LoadFoodsCommand.Execute(null);
    }
}