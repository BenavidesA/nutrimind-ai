using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Models.MealPlans;
using NutriMind.Mobile.Services.Api;

namespace NutriMind.Mobile.ViewModels.MealPlans;

[QueryProperty(nameof(EditingPlan), "MealPlanData")]
public partial class AddMealPlanViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    public AddMealPlanViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [ObservableProperty]
    private MealPlanResponseDto? editingPlan;

    [ObservableProperty]
    private bool isEditMode;

    [ObservableProperty]
    private string pageTitle = "Nuevo Plan";

    [ObservableProperty]
    private string headerText = "Nuevo Plan de Alimentación";

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private DateTime startDate = DateTime.Today;

    [ObservableProperty]
    private DateTime endDate = DateTime.Today.AddDays(7);

    [ObservableProperty]
    private string calories = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    partial void OnEditingPlanChanged(MealPlanResponseDto? value)
    {
        if (value == null) return;

        IsEditMode = true;
        PageTitle = "Editar Plan";
        HeaderText = "Editar Plan de Alimentación";

        Name = value.Name;
        StartDate = value.StartDate;
        EndDate = value.EndDate;
        Calories = value.TotalCaloriesPerDay.ToString();
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Error", "El nombre del plan es obligatorio.", "OK");
            return;
        }

        if (EndDate < StartDate)
        {
            await Shell.Current.DisplayAlert("Error", "La fecha de fin no puede ser anterior a la fecha de inicio.", "OK");
            return;
        }

        if (!decimal.TryParse(Calories, out var caloriesValue) || caloriesValue <= 0)
        {
            await Shell.Current.DisplayAlert("Error", "Ingresa unas calorías por día válidas.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            bool success;
            if (IsEditMode && EditingPlan != null)
            {
                var dto = new UpdateMealPlanDto
                {
                    Name = Name,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    TotalCaloriesPerDay = caloriesValue
                };
                success = await _apiService.UpdateMealPlanAsync(EditingPlan.Id, dto);
            }
            else
            {
                var dto = new CreateMealPlanDto
                {
                    Name = Name,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    TotalCaloriesPerDay = caloriesValue,
                    PlannedMeals = new()
                };
                success = await _apiService.CreateMealPlanAsync(dto);
            }

            if (success)
            {
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                var verb = IsEditMode ? "actualizar" : "crear";
                await Shell.Current.DisplayAlert("Error", $"No se pudo {verb} el plan de alimentación.", "OK");
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
