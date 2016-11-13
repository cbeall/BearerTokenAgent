using BearerTokenAgent.RemoteToken;
using IdentityModel.Client;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BearerTokenAgent.ClientCredentials
{
    public class ClientCredentialsToken : IRemoteToken
    {
        private readonly ClientCredentialsTokenOptions _options;
        private readonly TokenClient _tokenClient;

        public ClientCredentialsToken(ClientCredentialsTokenOptions options)
        {
            _options = options;

            _tokenClient = new TokenClient(options.TokenServiceAddress,
               options.ClientId,
               options.ClientSecret,
               options.TokenServiceHttpMessageHandler ?? new HttpClientHandler(),
               AuthenticationStyle.BasicAuthentication);
        }

        public Task<TokenResponse> GetTokenAsync(CancellationToken cancellationToken)
        {
            var scopes = string.Join(" ", _options.Scopes);
            return _tokenClient.RequestClientCredentialsAsync(scopes, cancellationToken: cancellationToken);
        }

        public RemoteTokenOptions Options { get { return _options; } }

        public void Dispose()
        {
            _tokenClient.Dispose();
        }
    }
}
