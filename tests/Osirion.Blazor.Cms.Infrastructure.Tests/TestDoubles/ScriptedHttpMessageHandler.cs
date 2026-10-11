using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// Answers each request with the response a test builds for it (any status code, body or header) and records the
/// method, URL and body of every request it receives.
/// </summary>
internal sealed class ScriptedHttpMessageHandler : HttpMessageHandler
{
    private readonly ConcurrentQueue<ScriptedRequest> _requests = new();
    private readonly Func<HttpRequestMessage, HttpResponseMessage> _respond;

    public ScriptedHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        _respond = respond;
    }

    public IReadOnlyList<ScriptedRequest> Requests => _requests.ToArray();

    public static HttpResponseMessage Json(string body, HttpStatusCode status = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var body = request.Content is null ? null : await request.Content.ReadAsStringAsync(cancellationToken);
        _requests.Enqueue(new ScriptedRequest(request.Method, request.RequestUri!, body));

        return _respond(request);
    }
}

/// <summary>
/// One request as <see cref="ScriptedHttpMessageHandler"/> received it.
/// </summary>
internal sealed record ScriptedRequest(HttpMethod Method, Uri Uri, string? Body);
