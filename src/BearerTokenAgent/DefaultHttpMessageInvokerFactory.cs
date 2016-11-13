using System;
using System.Net.Http;

namespace BearerTokenAgent
{
    internal class DefaultHttpMessageInvokerFactory : IHttpMessageInvokerFactory
    {
        private readonly Func<HttpMessageHandler> _handlerFactory;

        public DefaultHttpMessageInvokerFactory()
        {
            _handlerFactory = CreateDefaultHttpClientHandler;
        }

        public DefaultHttpMessageInvokerFactory(HttpMessageHandler handler)
        {
            _handlerFactory = () => handler;
        }

        public HttpMessageInvoker CreateHttpMessageInvoker()
        {
            var handler = _handlerFactory();
            return new HttpMessageInvoker(handler, true);
        }

        private static HttpMessageHandler CreateDefaultHttpClientHandler()
        {
            return new HttpClientHandler();
        }
    }
}
