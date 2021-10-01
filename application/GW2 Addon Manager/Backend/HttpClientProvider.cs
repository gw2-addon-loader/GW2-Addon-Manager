using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GW2AddonManager
{
    interface IHttpClientProvider
    {
        HttpClient Client { get; }
    }

    class HttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient _httpClient;

        public HttpClient Client => _httpClient;

        public HttpClientProvider()
        {
            _httpClient = new HttpClient();

            var name = typeof(Utils).Assembly.GetName();
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(name.FullName, $"{name.Version.Major}.{name.Version.Minor}.{name.Version.Build}"));
        }
    }
}
