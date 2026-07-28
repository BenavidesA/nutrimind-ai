using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Tests.TestHelpers;

// NSubstitute no puede sustituir HttpMessageHandler.SendAsync directamente (es protected),
// así que se usa esta implementación real que delega en un delegado configurable por test.
// Permite simular tanto respuestas HTTP (200, 429, 500...) como fallos de red/timeout.
internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _responder;

    public FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responder)
    {
        _responder = responder;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_responder(request));
    }
}
