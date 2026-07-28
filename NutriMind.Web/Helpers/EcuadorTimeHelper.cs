namespace NutriMind.Web.Helpers;

// Copy of the equivalent helper in NutriMind.Application/Common/EcuadorTimeHelper.cs (and its
// duplicate in NutriMind.Mobile/Helpers) — Web doesn't reference Application either (it's a
// decoupled project, only consuming the API over HTTP). Ecuador uses a fixed UTC-5 offset
// year-round (no DST observed), so adding/subtracting fixed hours is safe.
public static class EcuadorTimeHelper
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(-5);

    public static DateTime ToLocal(DateTime utc) => utc.Add(Offset);

    // Converts a calendar day in Ecuador time (the time-of-day part is ignored) to the exact UTC
    // instant when that day starts (midnight Ecuador = 05:00 UTC). Must be used to build
    // any LogDate coming from an <input type="date"> before sending it to the API — the API
    // stores LogDate as raw UTC and scopes its per-day queries using this same conversion
    // (see NutriMind.Application/Common/EcuadorTimeHelper.EcuadorDayStartToUtc), so
    // relabeling the chosen date as UTC without adding the offset would shift it back a day.
    public static DateTime EcuadorDayStartToUtc(DateTime ecuadorLocalDate) => ecuadorLocalDate.Date - Offset;

    // Actual MealType IDs (seed data, NutriMind.Infrastructure/Persistence/Configurations/
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
