using System.Net.Http.Headers;

namespace NutriMind.Web.Services;

// Adjunta el AccessToken guardado como claim en la cookie de sesión (ver
// AuthController.SignInUserAsync) a cada request saliente hacia NutriMind.API.
// Equivalente web de NutriMind.Mobile/Services/Api/AuthHeaderHandler.cs — allá el token sale de
// ISecureStorageService, acá sale del HttpContext autenticado vía IHttpContextAccessor porque
// Web usa cookie auth, no hay almacenamiento local propio.
public class AuthTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var accessToken = _httpContextAccessor.HttpContext?.User.FindFirst("access_token")?.Value;
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
