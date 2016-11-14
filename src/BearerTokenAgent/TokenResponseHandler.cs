using BearerTokenAgent.RemoteToken;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BearerTokenAgent
{
    public class TokenResponseHandler : ITokenResponseHandler
    {
        private RemoteTokenOptions _options;

        public void EnsureTokenResponseSuccessful(TokenResponse tokenResponse, Func<bool> tokenResponseCancellationRequested)
        {
            if (_options == default(RemoteTokenOptions))
            {
                throw new InvalidOperationException(
                    $"Cannot ensure token response without {nameof(RemoteTokenOptions)} first being set");
            }

            if (tokenResponse.IsError)
            {
                if (tokenResponse.Exception is OperationCanceledException)
                {
                    if (tokenResponseCancellationRequested())
                    {
                        //TODO Token Service Time
                        throw new NotImplementedException("need to implement token response cancelled");
                    }

                    // A cancellation occured that was not the token response timeout cancellation. 
                    // Throw this cancellation directly so it can be caught upstream
                    throw tokenResponse.Exception;
                }

                throw new TokenResponseException(tokenResponse, _options.ClientId);
            }
        }

        public void SetRemoteTokenOptions(RemoteTokenOptions options)
        {
            _options = options;
        }
    }
}
