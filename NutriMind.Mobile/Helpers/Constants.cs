namespace NutriMind.Mobile.Helpers;

public static class Constants
{
#if ANDROID
    // El emulador de Android corre en su propia red virtual: "localhost" ahí apunta al
    // propio emulador, no a la máquina host. 10.0.2.2 es la IP especial (alias de Genymotion/
    // AVD) que redirige al localhost de Windows, donde docker-compose expone la API en el
    // puerto 8080 (HTTP, ver ASPNETCORE_URLS en docker-compose.yml).
    public const string BaseUrl = "http://10.0.2.2:8080/";
#else
    public const string BaseUrl = "http://localhost:8080/";
#endif
}