using System;
using System.Net.Http;

namespace BearerTokenAgent
{
    public class BearerTokenAgentOptions
    {
        public HttpMessageHandler InnerMessageHandler { get; set; }
        public bool DisposeInnerMessageHandler { get; set; } = true;

        public TimeSpan? MessageTimeout { get; set; }
    }
}
