using Common;
using Common.Utilities.ExternalApiBuilder;
using Microsoft.Extensions.Options;
using Services.Services.KavenegarServices.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Services.Services.KavenegarServices
{
    public class KavenegarHttpClient : BaseHttpClientWithFactory
    {
        private readonly KavenegarOptions _opt;

        protected override string ClientName => _opt.HttpClientName;

        public KavenegarHttpClient(IHttpClientFactory factory, IOptions<KavenegarOptions> options)
            : base(factory)
        {
            _opt = options.Value;

            var baseApi = new ApiBuilder(_opt.BaseUrl)
                .AddToPath(_opt.ApiKey ?? throw new ArgumentNullException(nameof(_opt.ApiKey)))
                .AddToPath(string.Empty)
                .GetUri();

            BaseAddress = baseApi;
            BasePath = "";
        }

        protected override JsonSerializerOptions JsonOptions { get; } = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
}