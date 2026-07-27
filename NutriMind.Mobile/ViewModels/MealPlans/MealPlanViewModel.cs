using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Models.MealPlans;
using NutriMind.Mobile.Services.Api;

namespace NutriMind.Mobile.ViewModels.MealPlans;

public partial class MealPlanViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private bool isBusy;

    public ObservableCollection<MealPlanResponseDto> MealPlans { get; } = new();

    public MealPlanViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private async Task LoadMealPlansAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            MealPlans.Clear();

            var result = await _apiService.GetMealPlansAsync();
            foreach (var plan in result)
            {
                MealPlans.Add(plan);
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToAddMealPlanAsync() => await Shell.Current.GoToAsync("addmealplan");

    [RelayCommand]
    private async Task EditMealPlanAsync(MealPlanResponseDto plan)
    {
        if (plan is null) return;

        var navParam = new Dictionary<string, object> { { "MealPlanData", plan } };
        await Shell.Current.GoToAsync("addmealplan", navParam);
    }

    [RelayCommand]
    private async Task DeleteMealPlanAsync(MealPlanResponseDto plan)
    {
        if (plan is null) return;

        MealPlans.Remove(plan);

        try
        {
            await _apiService.DeleteMealPlanAsync(plan.Id);
        }
        catch
        {
            MealPlans.Add(plan);
            await Shell.Current.DisplayAlert("Error", "No se pudo eliminar el plan del servidor.", "OK");
        }
    }
}
