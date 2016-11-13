using System.Net.Http;

namespace BearerTokenAgent
{
    public class BearerTokenAgentHttpMessageInvokerFactory : IHttpMessageInvokerFactory
    {
        private readonly BearerTokenAgentOptions _options;

        public BearerTokenAgentHttpMessageInvokerFactory(BearerTokenAgentOptions options)
        {
            _options = options;
        }

        public HttpMessageInvoker CreateHttpMessageInvoker()
        {
            var handler = _options.InnerMessageHandler ?? new HttpClientHandler();
            var disposeHandler = _options.InnerMessageHandler == null || _options.DisposeInnerMessageHandler;
            return new HttpMessageInvoker(handler, disposeHandler);
        }
    }
}
