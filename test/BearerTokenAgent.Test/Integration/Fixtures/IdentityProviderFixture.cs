using IdentityServer4.Models;
using Mendham.Testing.AspNetCore;
using Mendham.Testing.Moq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BearerTokenAgent.Test.Integration.Fixtures
{
    public class IdentityProviderFixture : TestServerFixture
    {
        public IMiddlewareAction MiddlewareAction { get; set; }

        public string GetTokenServiceAddress()
        {
            var uriBuilder = new UriBuilder(Server.BaseAddress);
            uriBuilder.Path = PathString.FromUriComponent(Server.BaseAddress)
                .Add("/connect/token");
            return uriBuilder.ToString();
        }

        public HttpMessageHandler CreateMessageHandler()
        {
            return Server.CreateHandler();
        }

        protected override IWebHostBuilder GetWebHostBuilder()
        {
            return new WebHostBuilder()
                .ConfigureServices(ConfigureServices)
                .Configure(Configure);
        }

        public override void ResetFixture()
        {
            MiddlewareAction = Mock.Of<IMiddlewareAction>();

            MiddlewareAction.AsMock()
                .Setup(a => a.TakeAction(It.IsAny<HttpContext>()))
                .ReturnsTask();
        }

        private void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(a => MiddlewareAction);

            serviceCollection.AddIdentityServer()
                .AddInMemoryScopes(GetScopes())
                .AddInMemoryClients(GetClients())
                .AddInMemoryPersistedGrants()
                .AddTemporarySigningCredential();
        }

        private void Configure(IApplicationBuilder app)
        {
            app.Use(next => async ctx =>
            {
                var middlewareAction = ctx.RequestServices.GetRequiredService<IMiddlewareAction>();
                await middlewareAction.TakeAction(ctx);
                await next(ctx);
            });

            app.UseIdentityServer();
        }

        private static IEnumerable<Scope> GetScopes()
        {
            yield return new Scope
            {
                Name = Constants.Scopes.Api1,
                Enabled = true,
                Type = ScopeType.Resource
            };
            yield return new Scope
            {
                Name = Constants.Scopes.Api2,
                Enabled = true,
                Type = ScopeType.Resource
            };
        }

        private static IEnumerable<Client> GetClients()
        {
            yield return new Client
            {
                ClientId = Constants.ClientCredentials.ClientId,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets =
                {
                    new Secret(Constants.ClientCredentials.ClientSecret.Sha256())
                },
                AllowedScopes =
                {
                    Constants.Scopes.Api1,
                    Constants.Scopes.Api2
                }
            };
        }
    }
}
