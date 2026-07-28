namespace NutriMind.Mobile.Helpers;

// Copy of the equivalent helper in NutriMind.Application/Common/EcuadorTimeHelper.cs — Mobile does
// not reference Application (decoupled project, only consumes the API over HTTP), so the class
// cannot be shared directly. Ecuador uses a fixed UTC-5 offset year-round (it does not
// observe daylight saving time/DST), so adding/subtracting fixed hours is safe.
public static class EcuadorTimeHelper
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(-5);

    public static DateTime ToLocal(DateTime utc) => utc.Add(Offset);

    // Auto-detects the meal type based on Ecuador local time at the moment of logging.
    // Actual MealTypes IDs (seed data, NutriMind.Infrastructure/Persistence/Configurations/
    // MealTypeConfiguration.cs): 1=Breakfast, 2=Lunch, 3=Dinner, 4=Snack.
    public static int GetMealTypeIdForNow()
    {
        var hour = ToLocal(DateTime.UtcNow).Hour;
        return hour switch
        {
            >= 5 and < 11 => 1,  // Breakfast
            >= 11 and < 16 => 2, // Lunch
            >= 16 and < 21 => 3, // Dinner
            _ => 4                // Snack
        };
    }
}
