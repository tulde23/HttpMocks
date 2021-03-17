using System.Threading.Tasks;

namespace System.Net.Http
{
    public static class HttpResponseMessageExtensions
    {
        /// <summary>
        /// Gets the content as string asynchronous.
        /// </summary>
        /// <param name="httpResponseMessage">The HTTP response message.</param>
        /// <returns></returns>
        public static Task<string> GetContentAsStringAsync(this HttpResponseMessage httpResponseMessage)
        {
            return httpResponseMessage?.Content?.ReadAsStringAsync();
        }
    }
}