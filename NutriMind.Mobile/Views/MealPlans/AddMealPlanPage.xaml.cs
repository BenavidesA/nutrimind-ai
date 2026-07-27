using NutriMind.Mobile.ViewModels.MealPlans;

namespace NutriMind.Mobile.Views.MealPlans;

public partial class AddMealPlanPage : ContentPage
{
    public AddMealPlanPage(AddMealPlanViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
