namespace NutriMind.Mobile.Models
{
    public class AiMealPlanRequest
    {
        public int TargetCalories { get; set; }
        public int Days { get; set; }
        public List<string> Allergies { get; set; } = new();
        public string DietType { get; set; } = string.Empty;
    }
}