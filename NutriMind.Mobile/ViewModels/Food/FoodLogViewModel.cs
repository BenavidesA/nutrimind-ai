using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Models.Food;
using NutriMind.Mobile.Services.Api; // <-- Uncommented

namespace NutriMind.Mobile.ViewModels.Food;

public partial class FoodLogViewModel : ObservableObject
{
    private readonly IApiService _apiService; // <-- Now the real thing

    // Actual MealTypes IDs (seed data in MealTypeConfiguration.cs): 1=Breakfast, 2=Lunch,
    // 3=Dinner, 4=Snack. Grouped by ascending Id, which already gives the correct chronological order.
    private static readonly Dictionary<int, string> MealTypeNames = new()
    {
        { 1, "Desayuno" },
        { 2, "Almuerzo" },
        { 3, "Cena" },
        { 4, "Snack" }
    };

    [ObservableProperty]
    private bool isBusy;

    public ObservableCollection<FoodGroup> GroupedFoods { get; } = new();

    public FoodLogViewModel(IApiService apiService)
    {
        _apiService = apiService;
        _ = LoadFoodsAsync();
    }


    [RelayCommand]
    private async Task LoadFoodsAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            GroupedFoods.Clear();

            var result = await _apiService.GetFoodsAsync();
            var groups = result
                .GroupBy(f => f.MealTypeId)
                .OrderBy(g => g.Key)
                .Select(g => new FoodGroup(MealTypeNames.TryGetValue(g.Key, out var name) ? name : "Otro", g));

            foreach (var group in groups)
            {
                GroupedFoods.Add(group);
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
    private async Task ViewFoodDetailsAsync(FoodItemDto food)
    {
        if (food is null) return;
        var navParam = new Dictionary<string, object> { { "FoodData", food } };
        await Shell.Current.GoToAsync("fooddetail", navParam);
    }

    [RelayCommand]
    private async Task DeleteFoodAsync(FoodItemDto food)
    {
        if (food is null) return;

        // Optimistic delete: remove the item from its group (or the entire group if it ends up
        // empty) by replacing the FoodGroup instance so the ObservableCollection notifies the
        // change — FoodGroup is a plain List<T>, it does not react to in-place mutations.
        var group = GroupedFoods.FirstOrDefault(g => g.Contains(food));
        var groupIndex = group != null ? GroupedFoods.IndexOf(group) : -1;

        if (group != null)
        {
            var remainingItems = group.Where(f => f != food).ToList();
            if (remainingItems.Count > 0)
                GroupedFoods[groupIndex] = new FoodGroup(group.MealTypeName, remainingItems);
            else
                GroupedFoods.RemoveAt(groupIndex);
        }

        try
        {
            // Safely parse the string to a Guid to match the API signature
            if (Guid.TryParse(food.Id, out Guid foodGuid))
            {
                await _apiService.DeleteFoodAsync(foodGuid);
            }
        }
        catch
        {
            // If it failed, reload from the server instead of trying to manually reinsert
            // the item at the exact position/group.
            await LoadFoodsAsync();
            await Shell.Current.DisplayAlert("Error", "No se pudo borrar el elemento del servidor.", "OK");
        }
    }

    [RelayCommand]
    private async Task EditFoodAsync(FoodItemDto food)
    {
        if (food is null) return;

        var input = await Shell.Current.DisplayPromptAsync(
            "Editar cantidad", "Nueva cantidad en gramos:",
            initialValue: food.ServingSizeG.ToString(), keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(input) || !decimal.TryParse(input, out var quantity) || quantity <= 0)
            return;

        if (!Guid.TryParse(food.Id, out var foodLogId)) return;

        var dto = new UpdateFoodLogDto
        {
            MealTypeId = food.MealTypeId,
            LogDate = food.LogDate,
            QuantityG = quantity,
            Notes = food.Notes
        };

        var success = await _apiService.UpdateFoodLogAsync(foodLogId, dto);
        if (success)
            await LoadFoodsAsync();
        else
            await Shell.Current.DisplayAlert("Error", "No se pudo actualizar el registro.", "OK");
    }

    [RelayCommand]
    private async Task GoToAddFoodAsync() => await Shell.Current.GoToAsync("addfood");



}