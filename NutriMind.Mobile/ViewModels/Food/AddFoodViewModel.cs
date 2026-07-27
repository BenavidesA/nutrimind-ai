using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Helpers;
using NutriMind.Mobile.Models.Food;
using NutriMind.Mobile.Services.Api;
using System;
using System.Threading.Tasks;
namespace NutriMind.Mobile.ViewModels.Food;

public partial class AddFoodViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    public AddFoodViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string grams = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Grams))
        {
            await Shell.Current.DisplayAlert("Error", "El nombre y la cantidad en gramos son obligatorios.", "OK");
            return;
        }

        if (!decimal.TryParse(Grams, out var gramsValue) || gramsValue <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "Ingresa una cantidad válida en gramos.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            var dto = new SmartAddFoodLogDto
            {
                Name = Name,
                QuantityG = gramsValue,
                MealTypeId = EcuadorTimeHelper.GetMealTypeIdForNow(),
                LogDate = DateTime.UtcNow
            };

            var success = await _apiService.AddSmartFoodLogAsync(dto);
            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo estimar ni registrar el alimento. Intenta con un nombre más específico.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync() => await Shell.Current.GoToAsync("..");
}