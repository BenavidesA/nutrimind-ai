using System.Net.Http.Headers;
using NutriMind.Mobile.Services.Storage;

namespace NutriMind.Mobile.Services.Api;

public class AuthHeaderHandler : DelegatingHandler
{
    private readonly ISecureStorageService _storageService;

    public AuthHeaderHandler(ISecureStorageService storageService)
    {
        _storageService = storageService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // 1. Intentamos obtener el token guardado en el dispositivo
        var token = await _storageService.GetTokenAsync();

        // 2. Si el token existe, lo inyectamos en la cabecera de la petición
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 3. Continuamos con la petición de forma normal
        return await base.SendAsync(request, cancellationToken);
    }
}