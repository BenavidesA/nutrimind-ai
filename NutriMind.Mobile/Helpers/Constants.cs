namespace NutriMind.Mobile.Helpers;

public static partial class Constants
{
#if ANDROID
    // A physical phone can't reach "localhost" or 10.0.2.2 (that alias only exists inside the
    // emulator's virtual network) — it needs the dev machine's real LAN IP, and both devices
    // must be on the same Wi-Fi network. That IP is machine/network-specific, so it lives in
    // Constants.Local.cs (gitignored) instead of here — copy Constants.Local.cs.example to
    // create it. If you're testing on the emulator instead of a physical device, set
    // LocalDevHost to "10.0.2.2" there.
    public static readonly string BaseUrl = $"http://{LocalDevHost}:8080/";
#else
    public const string BaseUrl = "http://localhost:8080/";
#endif
}