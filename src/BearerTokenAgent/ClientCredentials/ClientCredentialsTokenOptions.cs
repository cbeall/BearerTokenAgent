using BearerTokenAgent.RemoteToken;
using System.Collections.Generic;

namespace BearerTokenAgent.ClientCredentials
{
    public class ClientCredentialsTokenOptions : RemoteTokenOptions
    {
        public string ClientSecret { get; set; }
        public List<string> Scopes { get; set; } = new List<string>();
    }
}
