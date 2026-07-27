using System;

namespace NutriMind.Application.DTOs.Foods
{
    public class FoodDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Brand { get; set; }
        public string? Barcode { get; set; }
        public string? CategoryName { get; set; }
        public decimal ServingSizeG { get; set; }
        public string? ServingUnit { get; set; }
        public decimal CaloriesPer100g { get; set; }
        public decimal ProteinPer100g { get; set; }
        public decimal CarbsPer100g { get; set; }
        public decimal FatPer100g { get; set; }
        public decimal FiberPer100g { get; set; }
        public decimal SugarPer100g { get; set; }
        public decimal SodiumPer100g { get; set; }
        public string? ImageUrl { get; set; }
        public string? Source { get; set; }
    }
}
