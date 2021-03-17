using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace System.Net.Http
{
    /// <summary>
    /// A helper class used to invoke a delegating handler.
    /// </summary>
    public class DelegatingHandlerInvoker
    {
        internal DelegatingHandlerInvoker()
        {
        }

        /// <summary>
        /// Gets or sets the handler.
        /// </summary>
        /// <value>
        /// The handler.
        /// </value>
        public DelegatingHandler Handler { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        /// <value>
        /// The messages.
        /// </value>
        public List<HttpRequestMessage> Messages { get; set; }

        /// <summary>
        /// Invokes this instance.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<HttpResponseMessage>> Invoke()
        {
            var invoker = new HttpMessageInvoker(this.Handler);
            List<HttpResponseMessage> responses = new List<HttpResponseMessage>();
            foreach (var message in Messages)
            {
                var r = await invoker.SendAsync(message, new CancellationToken());
                responses.Add(r);
            }
            return responses;
        }

        /// <summary>
        /// Invokes the single.
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponseMessage> InvokeSingle()
        {
            var invoker = new HttpMessageInvoker(this.Handler);
            return await invoker.SendAsync(Messages.First(), new CancellationToken());
        }
    }
}