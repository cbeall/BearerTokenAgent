using BearerTokenAgent.Cancellation;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace BearerTokenAgent
{
    public class BearerTokenAgentHttpMessageHandler : HttpMessageHandler
    {
        private const string BearerScheme = "Bearer";

        private readonly ICancellationManager _cancellationMananger;
        private readonly ITokenManager _tokenManager;
        private readonly HttpMessageInvoker _httpMessageInvoker;

        public BearerTokenAgentHttpMessageHandler(ITokenManager tokenManager, ICancellationManager cancellationMananger)
            : this(tokenManager, cancellationMananger, new DefaultHttpMessageInvokerFactory())
        { }

        public BearerTokenAgentHttpMessageHandler(ITokenManager tokenManager, ICancellationManager cancellationMananger,
            IHttpMessageInvokerFactory httpMessageInvokerFactory)
        {
            _tokenManager = tokenManager;
            _cancellationMananger = cancellationMananger;
            _httpMessageInvoker = httpMessageInvokerFactory.CreateHttpMessageInvoker();
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CancellationToken messageCancellationToken = _cancellationMananger.GetCancellationToken();
            CancellationTokenSource linkedCancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(messageCancellationToken, cancellationToken);

            try
            {
                string token = await _tokenManager.GetTokenAsync(linkedCancellationTokenSource.Token);

                request.Headers.Authorization = new AuthenticationHeaderValue(BearerScheme, token);

                return await _httpMessageInvoker.SendAsync(request, linkedCancellationTokenSource.Token);
            }
            catch (OperationCanceledException ex)
            {
                Exception wrappedException = _cancellationMananger.HandleCancellation(ex);
                throw wrappedException;
            }
        }

        protected override void Dispose(bool disposing)
        {
            (_tokenManager as IDisposable)?.Dispose();
            _httpMessageInvoker.Dispose();
            base.Dispose(disposing);
        }
    }
}
