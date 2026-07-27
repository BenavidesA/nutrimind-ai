using System;

namespace NutriMind.Application.Common;

// Ecuador usa un offset fijo de UTC-5 todo el año (no observa horario de verano/DST),
// por lo que sumar/restar horas fijas es seguro — no hace falta TimeZoneInfo con reglas de DST.
public static class EcuadorTimeHelper
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(-5);

    public static DateTime ToLocal(DateTime utc) => utc.Add(Offset);

    // Convierte un día calendario en hora Ecuador (la parte de hora se ignora) al instante UTC
    // exacto en que ese día empieza (medianoche Ecuador = 05:00 UTC). Se usa para acotar rangos
    // de consulta por día calendario de Ecuador contra columnas guardadas en UTC crudo.
    public static DateTime EcuadorDayStartToUtc(DateTime ecuadorLocalDate) => ecuadorLocalDate.Date - Offset;
}
