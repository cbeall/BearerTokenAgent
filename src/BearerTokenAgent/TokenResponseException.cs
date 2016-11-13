using IdentityModel.Client;
using System;

namespace BearerTokenAgent
{
    public class TokenResponseException : Exception
    {
        public string ClientId { get; }
        public TokenResponse TokenReponse { get; }

        public TokenResponseException(TokenResponse tokenResponse, string clientId)
            : base(tokenResponse.Error, tokenResponse.Exception)
        {
            TokenReponse = tokenResponse;
            ClientId = clientId;
        }

        // TODO Override Message
    }
}
