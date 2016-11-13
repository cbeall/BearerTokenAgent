using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BearerTokenAgent.RemoteToken
{
    public interface IRemoteToken : IDisposable
    {
        Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken);

        RemoteTokenOptions Options { get; }
    }
}
