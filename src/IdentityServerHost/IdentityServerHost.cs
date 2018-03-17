using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerHost
{
    public class IdentityServerHost : IWebHost
    {
        private const string IdentityServerHostConfigurationNull =
            "The IdentityServerHostConfiguration can not be null.";

        private static readonly IPAddress loopbackAddress = IPAddress.Loopback;
        private readonly IPEndPoint endpoint;

        private readonly IWebHost webHost;

        public Uri BaseAddress => new Uri($"http://{endpoint}");

        public IFeatureCollection ServerFeatures => webHost.ServerFeatures;

        public IServiceProvider Services => webHost.Services;

        public IdentityServerHost(IdentityServerHostConfiguration configuration, int port = 6262)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration), IdentityServerHostConfigurationNull);
            }

            endpoint = new IPEndPoint(loopbackAddress, port);

            webHost = new WebHostBuilder()
                .UseKestrel()
                .ConfigureServices(services => ConfigureServices(services, configuration))
                .Configure(Configure)
                .UseUrls(BaseAddress.ToString())
                .Build();
        }

        public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
            => webHost.StartAsync(cancellationToken);

        public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
            => webHost.StopAsync(cancellationToken);

        public void Dispose()
            => webHost.Dispose();

        public void Start()
            => webHost.Start();

        public HttpClient CreateClient() => new HttpClient(CreateHandler()) {BaseAddress = BaseAddress};

        public HttpMessageHandler CreateHandler() => new HttpClientHandler();

        private static void ConfigureServices(IServiceCollection services, IdentityServerHostConfiguration configuration)
        {
            var identityServerBuilder = services.AddIdentityServer()
                .AddInMemoryClients(configuration.Clients)
                .AddInMemoryIdentityResources(configuration.IdentityResources)
                .AddInMemoryApiResources(configuration.ApiResources)
                .AddDefaultSecretParsers()
                .AddDefaultSecretValidators()
                .AddDefaultEndpoints();

            if (configuration.SigningCredentials != null)
            {
                identityServerBuilder.AddSigningCredential(configuration.SigningCredentials);
            }
            else
            {
                identityServerBuilder.AddDeveloperSigningCredential();
            }
        }

        private static void Configure(IApplicationBuilder app)
        {
            app.UseIdentityServer();
        }
    }
}
