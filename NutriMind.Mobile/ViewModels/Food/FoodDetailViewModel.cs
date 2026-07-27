using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Models.Food;

namespace NutriMind.Mobile.ViewModels.Food;

[QueryProperty(nameof(Food), "FoodData")]
public partial class FoodDetailViewModel : ObservableObject
{
    // Cambia la línea de la propiedad por esta:
    [ObservableProperty]
    private FoodItemDto food = new();

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}