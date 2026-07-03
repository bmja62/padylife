using System.Net.Http.Json;
using System.Text.Json;

namespace Common.Utilities.ExternalApiBuilder;

public class BaseHttpClientWithFactory
{
    private readonly IHttpClientFactory _factory;

    protected virtual string ClientName => null;

    public Uri BaseAddress { get; init; }
    public string BasePath { get; init; }

    protected virtual JsonSerializerOptions JsonOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public BaseHttpClientWithFactory(IHttpClientFactory factory)
    {
        _factory = factory;
    }

    private HttpClient GetHttpClient()
    {
        var client = string.IsNullOrWhiteSpace(ClientName)
            ? _factory.CreateClient()
            : _factory.CreateClient(ClientName!);

        if (BaseAddress is not null)
            client.BaseAddress = BaseAddress;

        return client;
    }

    public virtual async Task<T?> SendRequestAsync<T>(
        HttpRequestMessage request,
        CancellationToken cancellationToken = default) where T : class
    {
        using var client = GetHttpClient();

        if (!string.IsNullOrWhiteSpace(BasePath) &&
            request.RequestUri is { IsAbsoluteUri: false })
        {
            var built = BuildRelativeUri(BasePath!, request.RequestUri!.ToString());
            request.RequestUri = built;
        }

        using var response = await client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        if (response.Content?.Headers.ContentLength == 0 ||
            response.StatusCode == System.Net.HttpStatusCode.NoContent)
            return null;

        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken);
        return result;
    }

    public static Uri BuildRelativeUri(string basePath, string relative)
    {
        var left = basePath.TrimEnd('/');
        var right = relative.TrimStart('/');
        return new Uri($"{left}/{right}", UriKind.Relative);
    }
}
