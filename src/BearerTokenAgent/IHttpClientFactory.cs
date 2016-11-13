using BearerTokenAgent.ClientCredentials;
using BearerTokenAgent.RemoteToken;
using System.Net.Http;

namespace BearerTokenAgent
{
    public interface IHttpClientFactory
    {
        HttpClient GetHttpClient(IRemoteToken remoteToken);
        HttpClient GetHttpClient(IRemoteToken remoteToken, HttpMessageHandler handler);

        HttpClient GetHttpClient(ITokenManager tokenManager);
        HttpClient GetHttpClient(ITokenManager tokenManager, HttpMessageHandler handler);

        HttpClient GetClientCredentialsHttpClient(ClientCredentialsTokenOptions options);
        HttpClient GetClientCredentialsHttpClient(ClientCredentialsTokenOptions options, HttpMessageHandler handler);
    }
}
