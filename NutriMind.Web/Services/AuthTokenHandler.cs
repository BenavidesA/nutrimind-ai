using System.Net.Http.Headers;

namespace NutriMind.Web.Services;

// Attaches the AccessToken stored as a claim in the session cookie (see
// AuthController.SignInUserAsync) to every outgoing request to NutriMind.API.
// Web equivalent of NutriMind.Mobile/Services/Api/AuthHeaderHandler.cs — there the token comes
// from ISecureStorageService, here it comes from the authenticated HttpContext via
// IHttpContextAccessor because Web uses cookie auth and has no local storage of its own.
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
