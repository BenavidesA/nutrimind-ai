using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Models.Food;
using NutriMind.Mobile.Services.Api; // <-- Descomentado

namespace NutriMind.Mobile.ViewModels.Food;

public partial class FoodLogViewModel : ObservableObject
{
    private readonly IApiService _apiService; // <-- Ya es real

    // IDs reales de MealTypes (seed data en MealTypeConfiguration.cs): 1=Breakfast, 2=Lunch,
    // 3=Dinner, 4=Snack. Se agrupa por Id ascendente, que ya da el orden cronológico correcto.
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

        // Borrado optimista: quitamos el item de su grupo (o el grupo entero si queda vacío)
        // reemplazando la instancia de FoodGroup para que el ObservableCollection notifique el
        // cambio — FoodGroup es un List<T> plano, no reacciona a mutaciones in-place.
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
            // Parseamos el string a Guid de forma segura para la firma de la API
            if (Guid.TryParse(food.Id, out Guid foodGuid))
            {
                await _apiService.DeleteFoodAsync(foodGuid);
            }
        }
        catch
        {
            // Si falló, recargamos desde el servidor en vez de intentar reinsertar el item a
            // mano en la posición/grupo exactos.
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