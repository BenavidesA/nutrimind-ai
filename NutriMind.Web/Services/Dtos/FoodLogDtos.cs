namespace NutriMind.Web.Services.Dtos;

public class CreateFoodLogDto
{
    public Guid FoodId { get; set; }
    public int MealTypeId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal QuantityG { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFoodLogDto
{
    public int MealTypeId { get; set; }
    public DateTime LogDate { get; set; }
    public decimal QuantityG { get; set; }
    public string? Notes { get; set; }
}

public class QuickAddFoodLogDto
{
    public string Name { get; set; } = string.Empty;
    public decimal Calories { get; set; }
    public decimal Carbs { get; set; }
    public decimal Protein { get; set; }
    public decimal Fat { get; set; }
    public int MealTypeId { get; set; } = 1;
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

public class SmartAddFoodLogDto
{
    public string Name { get; set; } = string.Empty;
    public decimal QuantityG { get; set; }
    public int MealTypeId { get; set; } = 1;
    public DateTime LogDate { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}

public class FoodLogResponseDto
{
    public Guid Id { get; set; }
    public DateTime LogDate { get; set; }
    public decimal QuantityG { get; set; }
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public string? Notes { get; set; }

    public Guid FoodId { get; set; }
    public string FoodName { get; set; } = string.Empty;

    public int MealTypeId { get; set; }
    public string MealTypeName { get; set; } = string.Empty;
}
