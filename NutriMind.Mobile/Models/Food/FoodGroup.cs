namespace NutriMind.Mobile.Models.Food;

// Grupo simple para el CollectionView agrupado de FoodLogPage — se reconstruye completo en
// cada carga, no reacciona a cambios in-place (no hace falta, ver FoodLogViewModel.LoadFoodsAsync).
public class FoodGroup : List<FoodItemDto>
{
    public string MealTypeName { get; }

    public FoodGroup(string mealTypeName, IEnumerable<FoodItemDto> items) : base(items)
    {
        MealTypeName = mealTypeName;
    }
}
