using BearerTokenAgent.Cancellation;
using Mendham.Testing.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;

namespace BearerTokenAgent.Test.Integration.Fixtures
{
    public class IntegrationFixture : ServiceProviderFixture
    {
        public IdentityProviderFixture IdentityProvider { get; }
        public TestApiFixture TestApi { get; }

        public IMessageCancellation MessageCancellation { get; set; }

        public IntegrationFixture()
        {
            IdentityProvider = new IdentityProviderFixture();

            TestApi = new TestApiFixture(IdentityProvider.Server.BaseAddress.ToString(),
                IdentityProvider.Server.CreateHandler);
        }

        public IHttpClientFactory HttpClientFactory
        {
            get { return Services.GetRequiredService<IHttpClientFactory>(); }
        }

        public override void ResetFixture()
        {
            IdentityProvider.ResetFixture();
            TestApi.ResetFixture();

            MessageCancellation = Mock.Of<IMessageCancellation>(
                ctx => ctx.GetCancellationToken() == CancellationToken.None);
        }

        protected override void ServiceConfiguration(IServiceCollection services)
        {
            services.AddTransient<IHttpClientFactory, HttpClientFactory>();
            services.AddTransient<ICancellationManager, CancellationManager>();
            services.AddTransient(a => MessageCancellation);
        }

        public override void Dispose()
        {
            base.Dispose();
            TestApi.Dispose();
            IdentityProvider.Dispose();
        }
    }
}
