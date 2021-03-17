using Newtonsoft.Json;

namespace System.Net.Http
{
    /// <summary>
    /// Builds mock http response messages.
    /// </summary>
    public class MockResponseBuilder
    {
        private readonly MockHttpClientBuilder mockHttpClientBuilder;
        private readonly MockHttpOperation httpOperation;

        internal MockResponseBuilder(MockHttpClientBuilder mockHttpClientBuilder, MockHttpOperation httpOperation)
        {
            this.mockHttpClientBuilder = mockHttpClientBuilder;
            this.httpOperation = httpOperation;
        }

        public MockHttpClientBuilder Returns(string content, System.Net.HttpStatusCode status = System.Net.HttpStatusCode.OK, string contentType = "application/json")
        {
            httpOperation.Response = new HttpResponseMessage(status);
            var c = new StringContent(content);
            // c.Headers.Add("content-type", contentType);
            httpOperation.Response.Content = c;

            return this.mockHttpClientBuilder;
        }

        public MockHttpClientBuilder Returns<T>(T model, System.Net.HttpStatusCode status = System.Net.HttpStatusCode.OK, string contentType = "application/json")
        {
            httpOperation.Response = new HttpResponseMessage(status);
            var c = new StringContent(JsonConvert.SerializeObject(model));
            //  c.Headers.Add("content-type", contentType);
            httpOperation.Response.Content = c;

            return this.mockHttpClientBuilder;
        }
    }
}