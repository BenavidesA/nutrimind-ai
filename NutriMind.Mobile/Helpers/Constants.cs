namespace NutriMind.Mobile.Helpers;

public static class Constants
{
#if ANDROID
    public const string BaseUrl = "https://localhost:7295/";
#else
    public const string BaseUrl = "https://localhost:7295/";
#endif
}