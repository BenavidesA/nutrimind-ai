namespace NutriMind.Mobile.Helpers;

public static class Constants
{
#if ANDROID
    // Un teléfono físico no tiene acceso a "localhost" ni a 10.0.2.2 (ese alias solo existe
    // dentro de la red virtual del emulador) — necesita la IP LAN real de la PC que corre
    // Docker, y ambos dispositivos deben estar en la misma red Wi-Fi. Esta IP la asigna el
    // router por DHCP y puede cambiar; si deja de conectar, corre "ipconfig" en la PC y
    // actualiza este valor. Si vuelves a probar en el emulador en vez de un teléfono físico,
    // cambia esto de nuevo a "http://10.0.2.2:8080/".
    public const string BaseUrl = "http://192.168.100.115:8080/";
#else
    public const string BaseUrl = "http://localhost:8080/";
#endif
}