using System.Buffers;
using System.Security.Cryptography;
using System.Text;

namespace Osirion.Blazor.Cms.Infrastructure.GitHub;

/// <summary>
/// Verifies the <c>X-Hub-Signature-256</c> header GitHub sends with every webhook delivery
/// for a hook that has a secret.
/// </summary>
/// <remarks>
/// GitHub signs the raw request body with HMAC-SHA256 keyed by the webhook secret and sends
/// <c>sha256=</c> followed by the lowercase hex digest. The digest is decoded to bytes and
/// compared in constant time so the comparison does not reveal how many bytes matched.
/// </remarks>
internal static class GitHubWebhookSignature
{
    /// <summary>
    /// The name of the header that carries the HMAC-SHA256 signature.
    /// </summary>
    internal const string HeaderName = "X-Hub-Signature-256";

    private const string Prefix = "sha256=";
    private const int DigestLength = 32;
    private const int HexDigestLength = DigestLength * 2;

    /// <summary>
    /// Decodes a signature header value into its digest bytes.
    /// </summary>
    /// <param name="headerValue">The raw header value.</param>
    /// <param name="digest">The decoded digest when the method returns <see langword="true"/>.</param>
    /// <returns><see langword="true"/> when the value is <c>sha256=</c> followed by exactly 64 hex characters.</returns>
    internal static bool TryParse(string? headerValue, out byte[] digest)
    {
        digest = [];

        if (headerValue is null ||
            headerValue.Length != Prefix.Length + HexDigestLength ||
            !headerValue.StartsWith(Prefix, StringComparison.Ordinal))
        {
            return false;
        }

        var decoded = new byte[DigestLength];
        var status = Convert.FromHexString(headerValue.AsSpan(Prefix.Length), decoded, out _, out var bytesWritten);
        if (status != OperationStatus.Done || bytesWritten != DigestLength)
        {
            return false;
        }

        digest = decoded;
        return true;
    }

    /// <summary>
    /// Determines whether <paramref name="expectedDigest"/> is the HMAC-SHA256 of
    /// <paramref name="payload"/> keyed by <paramref name="secret"/>.
    /// </summary>
    /// <param name="expectedDigest">The digest decoded from the signature header.</param>
    /// <param name="secret">The webhook secret, encoded as UTF-8 for the HMAC key.</param>
    /// <param name="payload">The exact bytes of the request body.</param>
    /// <returns><see langword="true"/> when the digests are equal.</returns>
    internal static bool Matches(ReadOnlySpan<byte> expectedDigest, string secret, ReadOnlySpan<byte> payload)
    {
        Span<byte> computed = stackalloc byte[DigestLength];
        HMACSHA256.HashData(Encoding.UTF8.GetBytes(secret), payload, computed);
        return CryptographicOperations.FixedTimeEquals(computed, expectedDigest);
    }
}
