using Common;
using Common.Utilities.ExternalApiBuilder;
using Microsoft.Extensions.Options;
using Services.Services.KavenegarServices.Dto;

namespace Services.Services.KavenegarServices;

public class KavenegarHelpers
{
    private readonly KavenegarOptions _opt;
    private readonly KavenegarHttpClient _client;

    public KavenegarHelpers(KavenegarHttpClient client, IOptions<KavenegarOptions> options)
    {
        _client = client;
        _opt = options.Value;
    }

    public static FormUrlEncodedContent ToForm(Dictionary<string, object> parameters)
    {
        IEnumerable<KeyValuePair<string, string>> Enumerate()
        {
            if (parameters == null) yield break;
            foreach (var kv in parameters)
                yield return new KeyValuePair<string, string>(kv.Key, kv.Value?.ToString());
        }
        return new FormUrlEncodedContent(Enumerate()!);
    }

    public static Uri BuildEndpoint(string relative) =>
        KavenegarHttpClient.BuildRelativeUri("", relative);

    public async Task<T> SendFormAsync<T>(string endpoint, Dictionary<string, object> parameters, CancellationToken ct)
        where T : class
    {

        try
        {

            var relative = endpoint.TrimStart('/');
            var absoluteUri = new Uri(_client.BaseAddress, relative);
            using var req = new HttpRequestBuilder(absoluteUri.ToString())
           .HttpMethod(HttpMethod.Post)
                .Content(ToForm(parameters))
                .GetHttpMessage();

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            if (_opt.DefaultTimeout > TimeSpan.Zero)
                cts.CancelAfter(_opt.DefaultTimeout);

            var result = await _client.SendRequestAsync<T>(req, cts.Token);
            if (result is null) throw new InvalidOperationException("Empty response from Kavenegar.");
            return result;
        }
        catch(Exception ex)
        {

            await Console.Out.WriteLineAsync(ex.Message);
            throw;
        }
    }

    public static long? ToUnixSeconds(DateTime? dt) =>
        dt is null ? null : new DateTimeOffset(DateTime.SpecifyKind(dt.Value, DateTimeKind.Local)).ToUnixTimeSeconds();

    public static void EnsureOk(KavenegarResponse<object> resp)
    {
        if (resp.Return is null || resp.Return.Status != 200)
            throw new HttpRequestException($"Kavenegar error: {resp.Return?.Status ?? 0} - {resp.Return?.Message ?? "No message"}");
    }
}
