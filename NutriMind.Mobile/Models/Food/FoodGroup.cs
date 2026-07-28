namespace NutriMind.Mobile.Models.Food;

// Simple group for FoodLogPage's grouped CollectionView — fully rebuilt on every load,
// doesn't react to in-place changes (not needed, see FoodLogViewModel.LoadFoodsAsync).
public class FoodGroup : List<FoodItemDto>
{
    public string MealTypeName { get; }

    public FoodGroup(string mealTypeName, IEnumerable<FoodItemDto> items) : base(items)
    {
        MealTypeName = mealTypeName;
    }
}
