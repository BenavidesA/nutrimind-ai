namespace NutriMind.Mobile.Models.Food;

public class UpdateFoodLogDto
{
    public int MealTypeId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal QuantityG { get; set; }
    public string? Notes { get; set; }
}
