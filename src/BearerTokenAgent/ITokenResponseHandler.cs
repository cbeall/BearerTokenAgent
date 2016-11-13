using BearerTokenAgent.RemoteToken;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BearerTokenAgent
{
    public interface ITokenResponseHandler
    {
        void EnsureTokenResponseSuccessful(TokenResponse tokenResponse, Func<bool> tokenResponseCancellationRequested);
        void SetRemoteTokenOptions(RemoteTokenOptions options);
    }
}
