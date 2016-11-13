using System;
using System.Threading;
using System.Threading.Tasks;

namespace BearerTokenAgent.RemoteToken
{
    public class RemoteTokenManager : ITokenManager, IDisposable
    {
        private readonly SemaphoreSlim _lock;
        private readonly IRemoteToken _remoteToken;
        private readonly ITokenResponseHandler _tokenResponseHandler;

        private string _token;
        private DateTimeOffset? _tokenExpiration;

        public RemoteTokenManager(IRemoteToken remoteToken)
            : this(remoteToken, new TokenResponseHandler())
        {
        }

        public RemoteTokenManager(IRemoteToken remoteToken, ITokenResponseHandler tokenResponseHandler)
        {
            _lock = new SemaphoreSlim(1, 1);

            _token = null;
            _tokenExpiration = null;
            _remoteToken = remoteToken;
            _tokenResponseHandler = tokenResponseHandler;
            _tokenResponseHandler.SetRemoteTokenOptions(_remoteToken.Options);
        }

        public Task<string> GetTokenAsync(CancellationToken cancellationToken)
        {
            if (HasValidToken())
            {
                return Task.FromResult(_token);
            }
            else
            {
                return GetNewTokenAsync(cancellationToken);
            }
        }

        private async Task<string> GetNewTokenAsync(CancellationToken cancellationToken)
        {
            // TODO Add circuit breaker?
            await _lock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (!HasValidToken())
                {
                    await GetNewTokenFromLockAsync(cancellationToken);
                }

                return _token;
            }
            finally
            {
                _lock.Release();
            }
        }

        private bool HasValidToken()
        {
            return _token != null
                && _tokenExpiration.HasValue
                && _tokenExpiration.Value.Subtract(_remoteToken.Options.ReloadTokenWithin) < DateTimeOffset.UtcNow;
        }

        private async Task GetNewTokenFromLockAsync(CancellationToken cancellationToken)
        {
            var getTokenTimeoutCancellationTokenSource = _remoteToken.Options.TokenServiceTimeout.HasValue ?
                new CancellationTokenSource(_remoteToken.Options.TokenServiceTimeout.Value) : new CancellationTokenSource();
            var allCancellationTokens = CancellationTokenSource
                .CreateLinkedTokenSource(cancellationToken, getTokenTimeoutCancellationTokenSource.Token).Token;

            _token = null;
            _tokenExpiration = null;

            var response = await _remoteToken.GetTokenAsync(allCancellationTokens);

            _tokenResponseHandler.EnsureTokenResponseSuccessful(response,
                () => getTokenTimeoutCancellationTokenSource.IsCancellationRequested);

            _token = response.AccessToken;
            _tokenExpiration = DateTimeOffset.UtcNow.AddSeconds(response.ExpiresIn);
        }

        public void Dispose()
        {
            _remoteToken.Dispose();
            _lock.Dispose();
        }
    }
}
