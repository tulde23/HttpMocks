using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTests.HttpMocking
{
    public class FakeDelegatingHandler : DelegatingHandler
    {
        public FakeDelegatingHandler()
        {
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var builder = new UriBuilder(request.RequestUri);
            var content = await request.Content.ReadAsStringAsync();
            builder.Host = "www.google.com";
            request.Headers.Add("x-identity", "unitTest");
            var response = await base.SendAsync(request, cancellationToken);
            var data = await response.Content.ReadAsStringAsync();
            response.Content = new StringContent(data + content);
            return response;
        }
    }
}