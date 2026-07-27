namespace NutriMind.Web.Helpers;

// Copia del helper equivalente en NutriMind.Application/Common/EcuadorTimeHelper.cs (y su
// duplicado en NutriMind.Mobile/Helpers) — Web tampoco referencia Application (proyecto
// desacoplado, solo consume la API por HTTP). Ecuador usa un offset fijo de UTC-5 todo el año
// (no observa horario de verano/DST), así que sumar/restar horas fijas es seguro.
public static class EcuadorTimeHelper
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(-5);

    public static DateTime ToLocal(DateTime utc) => utc.Add(Offset);

    // Convierte un día calendario en hora Ecuador (la parte de hora se ignora) al instante UTC
    // exacto en que ese día empieza (medianoche Ecuador = 05:00 UTC). Debe usarse para construir
    // cualquier LogDate que salga de un <input type="date"> antes de mandarlo a la API — la API
    // guarda LogDate en UTC crudo y acota sus consultas por día con esta misma conversión
    // (ver NutriMind.Application/Common/EcuadorTimeHelper.EcuadorDayStartToUtc), así que
    // re-etiquetar la fecha elegida como UTC sin sumar el offset la corre un día hacia atrás.
    public static DateTime EcuadorDayStartToUtc(DateTime ecuadorLocalDate) => ecuadorLocalDate.Date - Offset;

    // IDs reales de MealTypes (seed data, NutriMind.Infrastructure/Persistence/Configurations/
    // MealTypeConfiguration.cs): 1=Breakfast, 2=Lunch, 3=Dinner, 4=Snack.
    public static int GetMealTypeIdForNow()
    {
        var hour = ToLocal(DateTime.UtcNow).Hour;
        return hour switch
        {
            >= 5 and < 11 => 1,  // Desayuno
            >= 11 and < 16 => 2, // Almuerzo
            >= 16 and < 21 => 3, // Cena
            _ => 4                // Snack
        };
    }
}
