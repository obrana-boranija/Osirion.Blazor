using System.Collections.Concurrent;
using System.Net;
using System.Text;

namespace Osirion.Blazor.Cms.Infrastructure.Tests.TestDoubles;

/// <summary>
/// Records every request it receives and answers 200 OK with a JSON body, so GitHub clients can be tested
/// without the network. The recorded headers are the ones the request carried on the wire, after
/// <see cref="HttpClient"/> merged its default headers.
/// </summary>
internal sealed class RecordingHttpMessageHandler : HttpMessageHandler
{
    private readonly ConcurrentQueue<RecordedRequest> _requests = new();
    private readonly Func<HttpRequestMessage, string> _respond;

    public RecordingHttpMessageHandler(Func<HttpRequestMessage, string>? respond = null)
    {
        _respond = respond ?? DefaultResponse;
    }

    public IReadOnlyList<RecordedRequest> Requests => _requests.ToArray();

    public void Clear() => _requests.Clear();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _requests.Enqueue(RecordedRequest.From(request));

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_respond(request), Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }

    /// <summary>
    /// Answers with the smallest body each GitHub endpoint used by the clients can deserialize.
    /// </summary>
    private static string DefaultResponse(HttpRequestMessage request)
    {
        var path = request.RequestUri!.AbsolutePath;

        if (path == "/user")
        {
            return """{"login":"test-user"}""";
        }

        if (path.Contains("/git/refs/heads/", StringComparison.Ordinal))
        {
            return """{"ref":"refs/heads/main","object":{"sha":"base-sha","type":"commit"}}""";
        }

        if (path.EndsWith("/branches", StringComparison.Ordinal) || path.EndsWith("/repos", StringComparison.Ordinal))
        {
            return "[]";
        }

        return "{}";
    }
}

/// <summary>
/// A snapshot of one request as it reached the handler.
/// </summary>
internal sealed record RecordedRequest(
    HttpMethod Method,
    Uri Uri,
    string? AuthorizationScheme,
    string? AuthorizationParameter,
    string UserAgent,
    string Accept)
{
    public static RecordedRequest From(HttpRequestMessage request)
    {
        return new RecordedRequest(
            request.Method,
            request.RequestUri!,
            request.Headers.Authorization?.Scheme,
            request.Headers.Authorization?.Parameter,
            request.Headers.UserAgent.ToString(),
            request.Headers.Accept.ToString());
    }
}
