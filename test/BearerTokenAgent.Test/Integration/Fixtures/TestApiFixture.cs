using IdentityModel;
using Mendham;
using Mendham.Testing;
using Mendham.Testing.Moq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BearerTokenAgent.Test.Integration.Fixtures
{
    public class TestApiFixture : IFixture, IDisposable
    {
        private readonly string _authority;
        private readonly Func<HttpMessageHandler> _getIdentityProviderHandler;

        public TestServer Server { get; }
        public HttpClient Client { get; }

        public IMiddlewareAction MiddlewareAction { get; set; }

        public TestApiFixture(string authority, Func<HttpMessageHandler> getIdentityProviderHandler)
        {
            _authority = authority;
            _getIdentityProviderHandler = getIdentityProviderHandler;

            var builder = new WebHostBuilder()
               .ConfigureServices(ConfigureServices)
               .Configure(Configure);

            Server = new TestServer(builder);
            Client = Server.CreateClient();
        }

        public void ResetFixture()
        {
            MiddlewareAction = Mock.Of<IMiddlewareAction>();

            MiddlewareAction.AsMock()
                .Setup(a => a.TakeAction(It.IsAny<HttpContext>()))
                .ReturnsTask();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(a => MiddlewareAction);
            services.AddSingleton(UrlEncoder.Default);
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Policies.Api1Policy, new AuthorizationPolicyBuilder()
                    .RequireClaim(JwtClaimTypes.Scope, Constants.Scopes.Api1).Build());
            });
        }

        private void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = _authority,

                ScopeName = Constants.Scopes.Api1,
                AdditionalScopes = Constants.Scopes.Api2.AsSingleItemEnumerable(),
                AutomaticAuthenticate = true,

                IntrospectionDiscoveryHandler = _getIdentityProviderHandler(),
                IntrospectionBackChannelHandler = _getIdentityProviderHandler(),
                JwtBackChannelHandler = _getIdentityProviderHandler(),

                RequireHttpsMetadata = false
            });

            app.Use(next => async ctx =>
            {
                var middlewareAction = ctx.RequestServices.GetRequiredService<IMiddlewareAction>();
                await middlewareAction.TakeAction(ctx);
                await next(ctx);
            });

            app.Map(Constants.Paths.Secure, a =>
            {
                a.Run(async ctx =>
                {
                    var authorizationService = ctx.RequestServices.GetRequiredService<IAuthorizationService>();

                    if (await authorizationService.AuthorizeAsync(ctx.User, Constants.Policies.Api1Policy))
                    {
                        ctx.Response.StatusCode = 200;
                    }
                    else
                    {
                        ctx.Response.StatusCode = 403;
                    }
                });
            });

            app.Run(ctx =>
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.NoContent;
                return Task.FromResult(0);
            });
        }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
    }
}
