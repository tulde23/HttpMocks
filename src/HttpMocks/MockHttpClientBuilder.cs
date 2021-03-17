using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace System.Net.Http
{
    /// <summary>
    /// A Fluent builder to mock http client requests.
    /// </summary>
    public class MockHttpClientBuilder
    {
        /// <summary>
        /// The operation map.
        /// </summary>
        internal Dictionary<string, MockHttpOperation> _operationMap = new Dictionary<string, MockHttpOperation>();

        private readonly UriBuilder _baseAddress;
        private readonly HttpMethod _httpMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpClientBuilder" /> class.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        internal MockHttpClientBuilder(string baseAddress = "http://localhost", HttpMethod httpMethod = null)
        {
            _baseAddress = new UriBuilder(baseAddress);
            _httpMethod = httpMethod ?? HttpMethod.Post;
        }

        /// <summary>
        /// Builds the HttpClient.
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <returns></returns>
        public HttpClient BuildClient()
        {
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);

            foreach (var item in _operationMap)
            {
                // Setup the PROTECTED method to mock
                handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
                )
                // prepare the expected response of the mocked http call
                .ReturnsAsync(item.Value.Response)
                .Verifiable();
            }
            var client = new HttpClient(handlerMock.Object);
            client.BaseAddress = _baseAddress.Uri;
            return client;
        }

        /// <summary>
        /// Builds the delegating handler.
        /// </summary>
        /// <typeparam name="THandler">The type of the handler.</typeparam>
        /// <param name="handler">The handler.</param>
        /// <returns></returns>
        public DelegatingHandlerInvoker BuildDelegatingHandler<THandler>(Func<THandler> handler) where THandler : DelegatingHandler
        {
            var instance = handler();
            var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Loose);
            foreach (var item in _operationMap)
            {
                // Setup the PROTECTED method to mock
                handlerMock.Protected().Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                     item.Value.Request,
                  ItExpr.IsAny<CancellationToken>()
                )  // prepare the expected response of the mocked http call
            .ReturnsAsync(item.Value.Response)
            .Verifiable();
            }
            instance.InnerHandler = handlerMock.Object;
            return new DelegatingHandlerInvoker
            {
                Handler = instance,
                Messages = _operationMap.Values.Select(x => x.Request).ToList()
            };
        }

        /// <summary>
        /// Builds the IHttpClientFactory.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public IHttpClientFactory BuildFactory(string name = null)
        {
            var client = BuildClient();
            var httpClientFactory = new Mock<IHttpClientFactory>();
            if (string.IsNullOrEmpty(name))
            {
                httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(client);
            }
            else
            {
                httpClientFactory.Setup(x => x.CreateClient(name)).Returns(client);
            }

            return httpClientFactory.Object;
        }

        /// <summary>
        /// Mocks an http request for the given relative uri.
        /// </summary>
        /// <param name="uri">A relative URI. Can be null</param>
        /// <returns></returns>
        public MockRequestBuilder For(string uri = null)
        {
            uri = uri ?? "/";
            var operation = new MockHttpOperation(_baseAddress, uri ?? "", _httpMethod);
            if (_operationMap.ContainsKey(operation.Uri))
            {
                _operationMap[operation.Uri] = operation;
            }
            else
            {
                _operationMap.Add(operation.Uri, operation);
            }
            return new MockRequestBuilder(this, operation);
        }
    }
}