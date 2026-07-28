using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Tests.TestHelpers;

// NSubstitute can't substitute HttpMessageHandler.SendAsync directly (it's protected),
// so this real implementation is used instead, delegating to a per-test configurable delegate.
// It allows simulating both HTTP responses (200, 429, 500...) and network/timeout failures.
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
