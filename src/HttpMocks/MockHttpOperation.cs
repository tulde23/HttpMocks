using System;
using System.Net.Http;

namespace System.Net.Http
{
    /// <summary>
    /// Models an operation.
    /// </summary>
    public class MockHttpOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockHttpOperation"/> class.
        /// </summary>
        /// <param name="uriBuilder">The URI builder.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="method">The method.</param>
        internal MockHttpOperation(UriBuilder uriBuilder, string uri, HttpMethod method = null)
        {
            uriBuilder.Path = uri;
            Uri = uriBuilder.Uri.ToString();
            Method = method ?? HttpMethod.Post;
        }

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public HttpMethod Method { get; set; }

        /// <summary>
        /// Gets the URI.
        /// </summary>
        /// <value>
        /// The URI.
        /// </value>
        public string Uri { get; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>
        /// The request.
        /// </value>
        public HttpRequestMessage Request { get; set; }

        /// <summary>
        /// Gets or sets the response.
        /// </summary>
        /// <value>
        /// The response.
        /// </value>
        public HttpResponseMessage Response { get; set; }
    }
}