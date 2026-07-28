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
        // 1. Try to get the token stored on the device
        var token = await _storageService.GetTokenAsync();

        // 2. If the token exists, inject it into the request header
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        // 3. Continue with the request normally
        return await base.SendAsync(request, cancellationToken);
    }
}