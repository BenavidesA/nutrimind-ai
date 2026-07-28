using System.ComponentModel.DataAnnotations;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Models;

public class MealPlanListViewModel
{
    public List<MealPlanResponseDto> Plans { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

// No food/PlannedMeals selector — same pattern as NutriMind.Mobile/Views/MealPlans/
// AddMealPlanPage (simple form, created with an empty PlannedMeals, which the backend accepts).
public class CreateMealPlanViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre del plan")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime StartDate { get; set; } = DateTime.Today;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de fin")]
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(6);

    [Required(ErrorMessage = "Las calorías por día son obligatorias.")]
    [Range(1, 20000, ErrorMessage = "Ingresa un valor de calorías válido.")]
    [Display(Name = "Calorías por día")]
    public decimal TotalCaloriesPerDay { get; set; }

    public string? ErrorMessage { get; set; }
}

public class EditMealPlanViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre del plan")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de inicio")]
    public DateTime StartDate { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha de fin")]
    public DateTime EndDate { get; set; }

    [Required(ErrorMessage = "Las calorías por día son obligatorias.")]
    [Range(1, 20000, ErrorMessage = "Ingresa un valor de calorías válido.")]
    [Display(Name = "Calorías por día")]
    public decimal TotalCaloriesPerDay { get; set; }

    public string? ErrorMessage { get; set; }
}

public class GenerateMealPlanViewModel
{
    [Required(ErrorMessage = "Las calorías objetivo son obligatorias.")]
    [Range(1, 20000, ErrorMessage = "Ingresa un valor de calorías válido.")]
    [Display(Name = "Calorías objetivo por día")]
    public decimal TargetCalories { get; set; }

    [Required]
    [Range(1, 30, ErrorMessage = "Ingresa entre 1 y 30 días.")]
    [Display(Name = "Días")]
    public int Days { get; set; } = 1;

    [Display(Name = "Tipo de dieta")]
    public string DietType { get; set; } = "Cualquiera";

    [Display(Name = "Alergias (separadas por coma)")]
    public string? AllergiesText { get; set; }

    public string? ErrorMessage { get; set; }
}
