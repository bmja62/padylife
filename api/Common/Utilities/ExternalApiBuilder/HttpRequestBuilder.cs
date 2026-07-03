using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Common.Utilities.ExternalApiBuilder
{
    public class HttpRequestBuilder
    {
        private readonly HttpRequestMessage _request;
        private string _baseAddress;
        private readonly ApiBuilder _apiBuilder;

        public HttpRequestBuilder(string uri) : this(new ApiBuilder(uri)) { }

        public HttpRequestBuilder(ApiBuilder apiBuilder)
        {
            _request = new HttpRequestMessage();
            _apiBuilder = apiBuilder ?? throw new ArgumentNullException(nameof(apiBuilder));
            _baseAddress = _apiBuilder.GetLeftPart();
            _request.RequestUri = _apiBuilder.GetUri();
        }

        public HttpRequestBuilder AddToPath(string path)
        {
            _apiBuilder.AddToPath(path);
            _request.RequestUri = _apiBuilder.GetUri();
            return this;
        }

        public HttpRequestBuilder SetPath(string path)
        {
            _apiBuilder.SetPath(path);
            _request.RequestUri = _apiBuilder.GetUri();
            return this;
        }

        /// <summary>
        /// اگر ورودی به‌صورت "Bearer xxxxx" باشد، استاندارد ست می‌شود.
        /// در غیر این صورت همان مقدار به‌صورت raw اضافه می‌شود (بدون اعتبارسنجی).
        /// توصیه: از SetAuthorizationTokenInBoxino برای گنجاندن Bearer استفاده کن.
        /// </summary>
        public HttpRequestBuilder SetAuthorizationToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return this;

            // استاندارد: "Bearer <token>"
            const string bearerPrefix = "Bearer ";
            if (token.StartsWith(bearerPrefix, StringComparison.OrdinalIgnoreCase))
            {
                var pure = token.Substring(bearerPrefix.Length).Trim();
                _request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", pure);
            }
            else
            {
                // معادل رفتاری قبلی (ولی بدون Validate)
                _request.Headers.TryAddWithoutValidation("Authorization", token);
            }
            return this;
        }

        public HttpRequestBuilder SetAuthorizationTokenInBoxino(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                _request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            return this;
        }

        public HttpRequestBuilder SetApiTokenInBoxino(string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                _request.Headers.Remove("api-token");
                _request.Headers.Add("api-token", token);
            }
            return this;
        }

        public HttpRequestBuilder HttpMethod(HttpMethod httpMethod)
        {
            _request.Method = httpMethod ?? throw new ArgumentNullException(nameof(httpMethod));
            return this;
        }

        public HttpRequestBuilder Headers(Action<HttpRequestHeaders> funcOfHeaders)
        {
            funcOfHeaders?.Invoke(_request.Headers);
            return this;
        }

        public HttpRequestBuilder Headers(NameValueCollection headers)
        {
            if (headers is null) return this;

            // پاک‌سازی قبلی مطابق رفتار کد قدیم
            _request.Headers.Clear();

            foreach (var key in headers.AllKeys.Where(k => k is not null))
            {
                var val = headers[key!];
                if (string.Equals(key, "Authorization", StringComparison.OrdinalIgnoreCase))
                {
                    // تلاش برای تفسیر استاندارد bearer
                    SetAuthorizationToken(val!);
                }
                else
                {
                    _request.Headers.TryAddWithoutValidation(key!, val);
                }
            }
            return this;
        }

        public HttpRequestBuilder Content(HttpContent content)
        {
            _request.Content = content;
            return this;
        }

        public HttpRequestBuilder RequestUri(Uri uri)
        {
            _request.RequestUri = new ApiBuilder(uri.ToString()).GetUri();
            return this;
        }

        public HttpRequestBuilder RequestUri(string uri)
        {
            return RequestUri(new Uri(uri, UriKind.RelativeOrAbsolute));
        }

        public HttpRequestBuilder BaseAddress(string address)
        {
            _baseAddress = address;
            return this;
        }

        public HttpRequestBuilder Subdomain(string subdomain)
        {
            _apiBuilder.SetSubdomain(subdomain);
            _request.RequestUri = _apiBuilder.GetUri();
            return this;
        }

        public HttpRequestBuilder AddQueryString(string name, string value)
        {
            _apiBuilder.AddQueryString(name, value);
            _request.RequestUri = _apiBuilder.GetUri();
            return this;
        }

        public HttpRequestBuilder SetQueryString(string qs)
        {
            _apiBuilder.QueryString(qs);
            _request.RequestUri = _apiBuilder.GetUri();
            return this;
        }

        /// <summary>
        /// مالکیت HttpRequestMessage با caller است (Dispose بر عهده‌ی caller).
        /// اگر URI نسبی باشد و BaseAddress تنظیم شده باشد، اینجا Absolute می‌کنیم.
        /// </summary>
        public HttpRequestMessage GetHttpMessage()
        {
            var msg = _request;

            if (msg.RequestUri is not null &&
                !msg.RequestUri.IsAbsoluteUri &&
                !string.IsNullOrWhiteSpace(_baseAddress))
            {
                var baseUri = new Uri(_baseAddress!, UriKind.Absolute);
                msg.RequestUri = new Uri(baseUri, msg.RequestUri);
            }

            return msg;
        }

        public ApiBuilder GetApiBuilder()
        {
            return new ApiBuilder(_request.RequestUri?.ToString() ?? _baseAddress ?? string.Empty);
        }
    }
}
