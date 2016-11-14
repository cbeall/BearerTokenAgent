using BearerTokenAgent.Cancellation;
using BearerTokenAgent.ClientCredentials;
using BearerTokenAgent.RemoteToken;
using System.Net.Http;
using System;

namespace BearerTokenAgent
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly ICancellationManager _cancellationMananger;

        public HttpClientFactory(ICancellationManager cancellationMananger)
        {
            _cancellationMananger = cancellationMananger;
        }

        public HttpClient GetHttpClient(IRemoteToken remoteToken)
        {
            ITokenManager tokenManager = new RemoteTokenManager(remoteToken);
            return GetHttpClient(tokenManager);
        }

        public HttpClient GetHttpClient(IRemoteToken remoteToken, HttpMessageHandler handler)
        {
            ITokenManager tokenManager = new RemoteTokenManager(remoteToken);
            return GetHttpClient(tokenManager, handler);
        }

        public HttpClient GetHttpClient(ITokenManager tokenManager)
        {
            IHttpMessageInvokerFactory httpMessageInvokerFactory = new DefaultHttpMessageInvokerFactory();
            return GetHttpClient(tokenManager, httpMessageInvokerFactory);
        }

        public HttpClient GetHttpClient(ITokenManager tokenManager, HttpMessageHandler handler)
        {
            IHttpMessageInvokerFactory httpMessageInvokerFactory = new DefaultHttpMessageInvokerFactory(handler);
            return GetHttpClient(tokenManager, httpMessageInvokerFactory);
        }

        public HttpClient GetClientCredentialsHttpClient(ClientCredentialsTokenOptions options)
        {
            IRemoteToken remoteToken = new ClientCredentialsToken(options);
            return GetHttpClient(remoteToken);
        }

        public HttpClient GetClientCredentialsHttpClient(ClientCredentialsTokenOptions options, HttpMessageHandler handler)
        {
            IRemoteToken remoteToken = new ClientCredentialsToken(options);
            return GetHttpClient(remoteToken, handler);
        }

        private HttpClient GetHttpClient(ITokenManager tokenManager, IHttpMessageInvokerFactory httpMessageInvokerFactory)
        {
            var handler = new BearerTokenAgentHttpMessageHandler(
                tokenManager, _cancellationMananger, httpMessageInvokerFactory);
            return new HttpClient(handler);
        }
    }
}
