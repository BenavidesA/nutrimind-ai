using NutriMind.Mobile.ViewModels.Food;

namespace NutriMind.Mobile.Views.Food;

// AQUÍ ESTABA EL ERROR: Tiene que ser "public partial class", no "private"
public partial class FoodDetailPage : ContentPage
{
    public FoodDetailPage(FoodDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}