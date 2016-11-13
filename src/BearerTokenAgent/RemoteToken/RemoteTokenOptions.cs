using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BearerTokenAgent.RemoteToken
{
    public abstract class RemoteTokenOptions
    {
        public string TokenServiceAddress { get; set; }
        public string ClientId { get; set; }


        public HttpMessageHandler TokenServiceHttpMessageHandler { get; set; }

        public TimeSpan? TokenServiceTimeout { get; set; }

        /// <summary>
        /// The span of time prior to token expiration that will require the token to be replaced
        /// </summary>
        public TimeSpan ReloadTokenWithin { get; set; } = TimeSpan.FromMinutes(1);

    }
}
