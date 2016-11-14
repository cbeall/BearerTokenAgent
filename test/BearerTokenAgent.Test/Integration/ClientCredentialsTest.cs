using BearerTokenAgent.ClientCredentials;
using BearerTokenAgent.Test.Integration.Fixtures;
using FluentAssertions;
using Mendham.Testing;
using Mendham.Testing.Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace BearerTokenAgent.Test.Integration
{
    public class ClientCredentialsTest : Test<IntegrationFixture>
    {
        public ClientCredentialsTest(IntegrationFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task ClientCredentials_HasCorrectScope_Status200()
        {
            var options = new ClientCredentialsTokenOptions
            {
                ClientId = Constants.ClientCredentials.ClientId,
                ClientSecret = Constants.ClientCredentials.ClientSecret,
                Scopes = new List<string> { Constants.Scopes.Api1},

                TokenServiceAddress = Fixture.IdentityProvider.GetTokenServiceAddress(),
                TokenServiceHttpMessageHandler = Fixture.IdentityProvider.CreateMessageHandler()
            };

            using (var client = Fixture.HttpClientFactory
                .GetClientCredentialsHttpClient(options, Fixture.TestApi.Server.CreateHandler()))
            {
                client.BaseAddress = Fixture.TestApi.Server.BaseAddress;
                var response = await client.GetAsync(Constants.Paths.Secure);

                response.StatusCode.Should()
                    .Be(HttpStatusCode.OK, "api1 scope is inclued in bearer token");
            }
        }

        [Fact]
        public async Task ClientCredentials_DoesNotHaveCorrectScope_Status403()
        {
            var options = new ClientCredentialsTokenOptions
            {
                ClientId = Constants.ClientCredentials.ClientId,
                ClientSecret = Constants.ClientCredentials.ClientSecret,
                Scopes = new List<string> { Constants.Scopes.Api2 },

                TokenServiceAddress = Fixture.IdentityProvider.GetTokenServiceAddress(),
                TokenServiceHttpMessageHandler = Fixture.IdentityProvider.CreateMessageHandler()
            };

            using (var client = Fixture.HttpClientFactory
                .GetClientCredentialsHttpClient(options, Fixture.TestApi.Server.CreateHandler()))
            {
                client.BaseAddress = Fixture.TestApi.Server.BaseAddress;
                var response = await client.GetAsync(Constants.Paths.Secure);

                response.StatusCode.Should()
                    .Be(HttpStatusCode.Forbidden, "api1 scope was not included in bearer token");
            }
        }
    }
}