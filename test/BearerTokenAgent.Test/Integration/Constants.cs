using Microsoft.AspNetCore.Http;

namespace BearerTokenAgent.Test.Integration
{
    public static class Constants
    {
        public class ClientCredentials
        {
            public const string ClientId = "client_credentials";
            public const string ClientSecret = "abc";
        }


        public class Scopes
        {
            public const string Api1 = "api1";
            public const string Api2 = "api2";
        }

        public class Policies
        {
            public const string Api1Policy = "api1policy";
        }

        public class Paths
        {
            public static readonly PathString Secure = "/secure";
        }
    }
}
