using NutriMind.Mobile.ViewModels.Food;

namespace NutriMind.Mobile.Views.Food;

// Must be "public partial class" — the XAML-generated partial (FoodDetailPage.xaml.g.cs)
// declares this class as public, and C# requires matching accessibility across partial declarations.
public partial class FoodDetailPage : ContentPage
{
    public FoodDetailPage(FoodDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}