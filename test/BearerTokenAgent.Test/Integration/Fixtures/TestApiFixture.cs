using Mendham.Testing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
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
            MiddlewareAction = Mock.Of<IMiddlewareAction>(ctx => ctx.TakeAction() == Task.FromResult(0));
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(a => MiddlewareAction);
            services.AddSingleton(UrlEncoder.Default);
        }

        private void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = _authority,

                ScopeName = Constants.Scopes.Api1,
                AutomaticAuthenticate = true,

                IntrospectionDiscoveryHandler = _getIdentityProviderHandler(),
                IntrospectionBackChannelHandler = _getIdentityProviderHandler(),
                JwtBackChannelHandler = _getIdentityProviderHandler(),

                RequireHttpsMetadata = false
            });

            app.Use(next => async ctx =>
            {
                var middlewareAction = ctx.RequestServices.GetRequiredService<IMiddlewareAction>();
                await middlewareAction.TakeAction();
                await next(ctx);
            });
        }

        public void Dispose()
        {
            Client.Dispose();
            Server.Dispose();
        }
    }
}
