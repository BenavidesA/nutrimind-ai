namespace NutriMind.Mobile.Models.MealPlans;

public class CreateMealPlanDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalCaloriesPerDay { get; set; }
    public List<CreatePlannedMealDto> PlannedMeals { get; set; } = new();
}

public class CreatePlannedMealDto
{
    public DateTime Day { get; set; }
    public decimal QuantityG { get; set; }
    public int MealTypeId { get; set; }
    public Guid FoodId { get; set; }
}
