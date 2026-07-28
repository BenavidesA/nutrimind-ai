using NutriMind.Mobile.ViewModels.MealPlans;

namespace NutriMind.Mobile.Views.MealPlans;

public partial class MealPlanPage : ContentPage
{
    private readonly MealPlanViewModel _viewModel;

    // Parameterless constructor (fallback if Shell instantiates without going through DI)
    public MealPlanPage()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(MealPlanViewModel)) as MealPlanViewModel)!;

        BindingContext = _viewModel;
    }

    // Constructor with Dependency Injection (Primary)
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
