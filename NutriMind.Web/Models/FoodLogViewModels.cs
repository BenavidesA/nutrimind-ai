using System.ComponentModel.DataAnnotations;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Models;

public class FoodLogListViewModel
{
    public DateTime Date { get; set; }
    public List<FoodLogResponseDto> Logs { get; set; } = new();
    public decimal TotalCalories { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarbs { get; set; }
    public decimal TotalFat { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AddFoodLogViewModel
{
    [Required(ErrorMessage = "El nombre del alimento es obligatorio.")]
    [Display(Name = "Alimento")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(0.1, 10000, ErrorMessage = "Ingresa una cantidad válida en gramos.")]
    [Display(Name = "Cantidad (g)")]
    public decimal QuantityG { get; set; }

    [Required]
    [Display(Name = "Tipo de comida")]
    public int MealTypeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime Date { get; set; } = DateTime.Today;

    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    public string? ErrorMessage { get; set; }
}

public class EditFoodLogViewModel
{
    public Guid Id { get; set; }

    [Display(Name = "Alimento")]
    public string FoodName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(0.1, 10000, ErrorMessage = "Ingresa una cantidad válida en gramos.")]
    [Display(Name = "Cantidad (g)")]
    public decimal QuantityG { get; set; }

    [Required]
    [Display(Name = "Tipo de comida")]
    public int MealTypeId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [Display(Name = "Fecha")]
    public DateTime Date { get; set; }

    [Display(Name = "Notas")]
    public string? Notes { get; set; }

    public string? ErrorMessage { get; set; }
}

public static class MealTypes
{
    // Catálogo fijo (seed data, NutriMind.Infrastructure/Persistence/Configurations/
    // MealTypeConfiguration.cs): 1=Breakfast, 2=Lunch, 3=Dinner, 4=Snack.
    public static readonly Dictionary<int, string> Names = new()
    {
        { 1, "Desayuno" },
        { 2, "Almuerzo" },
        { 3, "Cena" },
        { 4, "Snack" }
    };
}
