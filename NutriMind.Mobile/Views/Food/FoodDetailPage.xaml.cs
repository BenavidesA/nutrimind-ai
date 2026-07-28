using NutriMind.Mobile.ViewModels.Food;

namespace NutriMind.Mobile.Views.Food;

// THIS WAS THE BUG: it has to be "public partial class", not "private"
public partial class FoodDetailPage : ContentPage
{
    public FoodDetailPage(FoodDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}