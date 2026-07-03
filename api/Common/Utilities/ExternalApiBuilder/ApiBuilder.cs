using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Http.Extensions; 
using Microsoft.Extensions.Primitives;

namespace Common.Utilities.ExternalApiBuilder
{
    public class ApiBuilder
    {
        private readonly string _fullUrl;
        private readonly UriBuilder _builder;

        public ApiBuilder(string url)
        {
            _fullUrl = url ?? throw new ArgumentNullException(nameof(url));
            _builder = new UriBuilder(url);
        }

        public Uri GetUri() => _builder.Uri;

        public ApiBuilder Scheme(string scheme)
        {
            _builder.Scheme = scheme;
            return this;
        }

        public ApiBuilder Host(string host)
        {
            _builder.Host = host;
            return this;
        }

        public ApiBuilder Port(int port)
        {
            _builder.Port = port;
            return this;
        }

        public ApiBuilder AddToPath(string path)
        {
            IncludePath(path);
            return this;
        }

        public ApiBuilder SetPath(string path)
        {
            _builder.Path = NormalizePath(path);
            return this;
        }

        public void IncludePath(string path)
        {
            var current = string.IsNullOrEmpty(_builder.Path) ? "/" : _builder.Path;
            var combined = $"{current.TrimEnd('/')}/{path?.TrimStart('/')}";
            _builder.Path = NormalizePath(combined);
        }

        public ApiBuilder Fragment(string fragment)
        {
            _builder.Fragment = fragment;
            return this;
        }

        public ApiBuilder SetSubdomain(string subdomain)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                throw new ArgumentException("Subdomain is required.", nameof(subdomain));

            var host = new Uri(_fullUrl).Host;
            _builder.Host = $"{subdomain}.{host}";
            return this;
        }

        public bool HasSubdomain()
        {
            return _builder.Uri.HostNameType == UriHostNameType.Dns
                   && _builder.Uri.Host.Split('.').Length > 2;
        }

        /// <summary>
        /// اگر کلید تکراری بود، مقدار جدید را با کاما به همان کلید اضافه می‌کند (مشابه رفتاری که قبلاً داشتی).
        /// </summary>
        public ApiBuilder AddQueryString(string name, string value)
        {
            var parsed = QueryHelpers.ParseQuery(_builder.Query ?? string.Empty);
            var builder = new QueryBuilder();

            foreach (var kv in parsed)
            {
                builder.Add(kv.Key, kv.Value.AsEnumerable());
            }

            if (parsed.TryGetValue(name, out StringValues existing) && !StringValues.IsNullOrEmpty(existing))
            {
                var combined = string.Join(",", existing.ToString(), value);
                var rebuilt = new QueryBuilder();
                foreach (var kv in parsed)
                {
                    if (!string.Equals(kv.Key, name, StringComparison.Ordinal))
                    {
                        rebuilt.Add(kv.Key, kv.Value.AsEnumerable());
                    }
                }
                rebuilt.Add(name, combined);
                _builder.Query = rebuilt.ToQueryString().Value?.TrimStart('?');
            }
            else
            {
                builder.Add(name, value);
                _builder.Query = builder.ToQueryString().Value?.TrimStart('?');
            }

            return this;
        }

        public ApiBuilder QueryString(string queryString)
        {
            if (!string.IsNullOrEmpty(queryString))
            {
                _builder.Query = queryString.TrimStart('?');
            }
            return this;
        }

        public ApiBuilder UserName(string username)
        {
            _builder.UserName = username;
            return this;
        }

        public ApiBuilder Password(string password)
        {
            _builder.Password = password;
            return this;
        }

        public string GetLeftPart()
        {
            return _builder.Uri.GetLeftPart(UriPartial.Path);
        }

        private static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return "/";
            var normalized = path.Replace("\\", "/");
            while (normalized.Contains("//"))
                normalized = normalized.Replace("//", "/");
            if (!normalized.StartsWith("/")) normalized = "/" + normalized;
            return normalized;
        }
    }
}
