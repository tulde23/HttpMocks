namespace System.Net.Http
{
    /// <summary>
    /// A factory class for mocking multiple aspects of the Http pipeline.
    /// You can mock an HttpClient, IHttpClientFactory and the invocation of a DelegatingHandler.
    /// </summary>
    public static class HttpMock
    {
        /// <summary>
        /// Mocks an http request given a uri and an http method
        /// </summary>
        /// <param name="baseAddress">The base address.</param>
        /// <param name="httpMethod">The HTTP method.  The default is POST</param>
        /// <returns></returns>
        public static MockHttpClientBuilder Mock(string baseAddress = "http://localhost", HttpMethod httpMethod = null)
        {
            return new MockHttpClientBuilder(baseAddress, httpMethod);
        }
    }
}