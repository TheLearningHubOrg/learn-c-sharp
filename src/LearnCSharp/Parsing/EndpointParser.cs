using System.Globalization;
using LearnCSharp.Models;

namespace LearnCSharp.Parsing;

public static class EndpointParser
{
    private const int MaxSchemeLength = 16;

    /// <summary>
    /// Parses "scheme://host[:port][/path]". Every intermediate value is a slice of
    /// the input span; strings are created only for the final NetworkEndpoint.
    /// </summary>
    public static Result<NetworkEndpoint> Parse(ReadOnlySpan<char> uri)
    {
        int schemeEnd = uri.IndexOf("://");
        if (schemeEnd <= 0)
        {
            return Result.Fail<NetworkEndpoint>("Expected scheme://host[:port].");
        }

        ReadOnlySpan<char> scheme = uri[..schemeEnd];
        if (scheme.Length > MaxSchemeLength)
        {
            return Result.Fail<NetworkEndpoint>("Scheme is too long.");
        }

        ReadOnlySpan<char> authority = uri[(schemeEnd + 3)..];
        int slash = authority.IndexOf('/');
        if (slash >= 0)
        {
            authority = authority[..slash];
        }

        int colon = authority.LastIndexOf(':');
        ReadOnlySpan<char> host = colon >= 0 ? authority[..colon] : authority;
        if (host.IsEmpty)
        {
            return Result.Fail<NetworkEndpoint>("Host is missing.");
        }

        // Lower-case the scheme into a small stack buffer: no heap allocation.
        Span<char> lowerScheme = stackalloc char[scheme.Length];
        scheme.ToLowerInvariant(lowerScheme);

        int port;
        if (colon >= 0)
        {
            ReadOnlySpan<char> portText = authority[(colon + 1)..];
            if (!int.TryParse(portText, NumberStyles.None, CultureInfo.InvariantCulture, out port)
                || port is < 1 or > 65_535)
            {
                return Result.Fail<NetworkEndpoint>("Port must be a number from 1 to 65535.");
            }
        }
        else
        {
            port = DefaultPort(lowerScheme);
            if (port == 0)
            {
                return Result.Fail<NetworkEndpoint>($"No default port for scheme '{scheme}'; add one explicitly.");
            }
        }

        return Result.Ok(new NetworkEndpoint(lowerScheme.ToString(), host.ToString(), port));
    }

    // Since C# 11 a span of char can be matched against constant strings.
    private static int DefaultPort(ReadOnlySpan<char> scheme) => scheme switch
    {
        "http" or "ws" => 80,
        "https" or "wss" => 443,
        "postgres" or "postgresql" => 5432,
        "redis" => 6379,
        _ => 0,
    };
}
