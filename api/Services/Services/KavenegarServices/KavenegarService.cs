using Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Services.Services.KavenegarServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services.KavenegarServices
{
    public class KavenegarService : IKavenegarService
    {
        private readonly ILogger<KavenegarService> _logger;
        private readonly KavenegarOptions _opt;
        private readonly KavenegarHelpers _helpers;

        public KavenegarService(
            ILogger<KavenegarService> logger,
            KavenegarHelpers helpers,
            IOptions<KavenegarOptions> options)
        {
            _logger = logger;
            _helpers = helpers;
            _opt = options.Value;
        }

        public async Task<SendResult> Send(string receptor, string message, string sender, CancellationToken ct)
        {
            var parameters = new Dictionary<string, object>
            {
                ["receptor"] = receptor,
                ["message"] = message,
                ["sender"] = sender ?? _opt.DefaultSender
            };

            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<SendResult>>>(
                KavenegarHelpers.BuildEndpoint("sms/send.json").ToString(), parameters, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            var item = resp.Entries?.FirstOrDefault() ?? new SendResult();
            _logger.LogInformation("SMS sent to {Receptor} status={Status} id={Id}", receptor, item.Status, item.Messageid);
            return item;
        }

        public async Task<List<SendResult>> SendArray(List<string> receptors, List<string> messages, string sender, CancellationToken ct)
        {
            if (receptors.Count != messages.Count)
                throw new ArgumentException("تعداد گیرندگان و پیام‌ها باید برابر باشد");

            var senders = Enumerable.Repeat(sender ?? _opt.DefaultSender, receptors.Count).ToList();

            var parameters = new Dictionary<string, object>
            {
                ["receptor"] = System.Text.Json.JsonSerializer.Serialize(receptors),
                ["message"] = System.Text.Json.JsonSerializer.Serialize(messages),
                ["sender"] = System.Text.Json.JsonSerializer.Serialize(senders)
            };

            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<SendResult>>>(
                KavenegarHelpers.BuildEndpoint("sms/sendarray.json").ToString(), parameters, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            _logger.LogInformation("Bulk SMS sent to {Count} recipients", receptors.Count);
            return resp.Entries ?? new List<SendResult>();
        }

        public async Task<StatusResult> Status(string messageId, CancellationToken ct)
        {
            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<StatusResult>>>(
                KavenegarHelpers.BuildEndpoint("sms/status.json").ToString(),
                new Dictionary<string, object> { ["messageid"] = messageId }, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            return resp.Entries?.FirstOrDefault() ?? new StatusResult();
        }

        public async Task<StatusResult> StatusByLocalId(string localId, CancellationToken ct)
        {
            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<StatusResult>>>(
                KavenegarHelpers.BuildEndpoint("sms/statuslocalmessageid.json").ToString(),
                new Dictionary<string, object> { ["localid"] = localId }, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            return resp.Entries?.FirstOrDefault() ?? new StatusResult();
        }

        public async Task<List<SendResult>> SelectOutbox(DateTime? startDate, DateTime? endDate, string sender, CancellationToken ct)
        {
            var p = new Dictionary<string, object>();
            var s = KavenegarHelpers.ToUnixSeconds(startDate);
            var e = KavenegarHelpers.ToUnixSeconds(endDate);
            if (s is not null) p["startdate"] = s;
            if (e is not null) p["enddate"] = e;
            if (!string.IsNullOrWhiteSpace(sender)) p["sender"] = sender;

            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<SendResult>>>(
                KavenegarHelpers.BuildEndpoint("sms/selectoutbox.json").ToString(), p, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            return resp.Entries ?? new List<SendResult>();
        }

        public async Task<List<SendResult>> LatestOutBox(int pageSize, string sender, CancellationToken ct)
        {
            var p = new Dictionary<string, object> { ["pagesize"] = pageSize };
            if (!string.IsNullOrWhiteSpace(sender)) p["sender"] = sender;

            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<SendResult>>>(
                KavenegarHelpers.BuildEndpoint("sms/latestoutbox.json").ToString(), p, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            return resp.Entries ?? new List<SendResult>();
        }

        public async Task<SendResult> Lookup(string receptor, string token, string template, CancellationToken ct)
        {
            var p = new Dictionary<string, object>
            {
                ["receptor"] = receptor,
                ["token"] = token,
                ["template"] = template
            };

            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<SendResult>>>(
                KavenegarHelpers.BuildEndpoint("verify/lookup.json").ToString(), p, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            return resp.Entries?.FirstOrDefault() ?? new SendResult();
        }

        public async Task<SendResult> SendWithTemplate(string receptor, string template, CancellationToken ct, params string[] tokenValues)
        {
            if (tokenValues.Length > 5) throw new ArgumentException("حداکثر 5 توکن پشتیبانی می‌شود");

            var p = new Dictionary<string, object> { ["receptor"] = receptor, ["template"] = template };
            var names = new[] { "token", "token2", "token3", "token10", "token20" };
            for (int i = 0; i < tokenValues.Length; i++)
                if (!string.IsNullOrEmpty(tokenValues[i])) p[names[i]] = tokenValues[i];

            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<SendResult>>>(
                KavenegarHelpers.BuildEndpoint("verify/lookup.json").ToString(), p, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            var item = resp.Entries?.FirstOrDefault() ?? new SendResult();
            _logger.LogInformation("Template SMS sent to {Receptor} template={Template} tokens={Count}",
                receptor, template, tokenValues.Length);
            return item;
        }

        public async Task<List<SendResult>> SendWithTemplateArray(List<string> receptors, string template, CancellationToken ct, params string[] tokenValues)
        {
            var tasks = receptors.Select(r => SendWithTemplate(r, template, ct, tokenValues));
            var results = await Task.WhenAll(tasks);
            _logger.LogInformation("Template SMS sent to {Count} recipients template={Template}", receptors.Count, template);
            return results.ToList();
        }

        public async Task<SendResult> TTS(string receptor, string message, CancellationToken ct)
        {
            var p = new Dictionary<string, object> { ["receptor"] = receptor, ["message"] = message };

            var resp = await _helpers.SendFormAsync<KavenegarResponse<List<SendResult>>>(
                KavenegarHelpers.BuildEndpoint("call/maketts.json").ToString(), p, ct);

            KavenegarHelpers.EnsureOk(new KavenegarResponse<object> { Return = resp.Return });
            var item = resp.Entries?.FirstOrDefault() ?? new SendResult();
            _logger.LogInformation("TTS call made to {Receptor} status={Status}", receptor, item.Status);
            return item;
        }

    }
}
