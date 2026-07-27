using NutriMind.Mobile.ViewModels.Food;

namespace NutriMind.Mobile.Views.Food;

public partial class AddFoodPage : ContentPage
{
    public AddFoodPage(AddFoodViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}