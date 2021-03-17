using Newtonsoft.Json;

namespace System.Net.Http
{
    /// <summary>
    /// Builds mock http request messages
    /// </summary>
    public class MockRequestBuilder
    {
        private const string DefaultContentType = "application/json";
        private readonly MockHttpClientBuilder _mockHttpClientBuilder;
        private readonly MockHttpOperation _httpOperation;

        internal MockRequestBuilder(MockHttpClientBuilder mockHttpClientBuilder, MockHttpOperation httpOperation)
        {
            _mockHttpClientBuilder = mockHttpClientBuilder;
            _httpOperation = httpOperation;
        }

        public MockResponseBuilder AcceptsAny()
        {
            _httpOperation.Request = new HttpRequestMessage(_httpOperation.Method, _httpOperation.Uri);
            var stringContent = new StringContent("{}");
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(DefaultContentType);
            _httpOperation.Request.Content = stringContent;
            return new MockResponseBuilder(_mockHttpClientBuilder, _httpOperation);
        }

        public MockResponseBuilder Accepts(string content, string contentType = DefaultContentType)
        {
            _httpOperation.Request = new HttpRequestMessage(_httpOperation.Method, _httpOperation.Uri);
            var stringContent = new StringContent(content);
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            _httpOperation.Request.Content = stringContent;
            return new MockResponseBuilder(_mockHttpClientBuilder, _httpOperation);
        }

        public MockResponseBuilder Accepts<T>(T model, string contentType = DefaultContentType)
        {
            _httpOperation.Request = new HttpRequestMessage(_httpOperation.Method, _httpOperation.Uri);

            var stringContent = new StringContent(JsonConvert.SerializeObject(model));
            stringContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
            _httpOperation.Request.Content = stringContent;
            return new MockResponseBuilder(_mockHttpClientBuilder, _httpOperation);
        }
    }
}