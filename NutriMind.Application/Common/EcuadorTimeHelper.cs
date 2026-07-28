using System;

namespace NutriMind.Application.Common;

// Ecuador uses a fixed UTC-5 offset year-round (it does not observe daylight saving time/DST),
// so adding/subtracting a fixed number of hours is safe — no need for TimeZoneInfo with DST rules.
public static class EcuadorTimeHelper
{
    private static readonly TimeSpan Offset = TimeSpan.FromHours(-5);

    public static DateTime ToLocal(DateTime utc) => utc.Add(Offset);

    // Converts a calendar day in Ecuador time (the time-of-day part is ignored) to the exact UTC
    // instant at which that day starts (midnight Ecuador = 05:00 UTC). Used to bound query ranges
    // by Ecuador calendar day against columns stored in raw UTC.
    public static DateTime EcuadorDayStartToUtc(DateTime ecuadorLocalDate) => ecuadorLocalDate.Date - Offset;
}
