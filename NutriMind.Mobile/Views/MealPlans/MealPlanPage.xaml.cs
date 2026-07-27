using NutriMind.Mobile.ViewModels.MealPlans;

namespace NutriMind.Mobile.Views.MealPlans;

public partial class MealPlanPage : ContentPage
{
    private readonly MealPlanViewModel _viewModel;

    // Constructor sin parámetros (fallback si Shell instancia sin pasar por DI)
    public MealPlanPage()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(MealPlanViewModel)) as MealPlanViewModel)!;

        BindingContext = _viewModel;
    }

    // Constructor con Inyección de Dependencias (Principal)
    public MealPlanPage(MealPlanViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.LoadMealPlansCommand.Execute(null);
    }
}
