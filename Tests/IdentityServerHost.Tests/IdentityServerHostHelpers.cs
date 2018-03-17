
using System.Threading.Tasks;
using IdentityModel.Client;

namespace IdentityServerHost.Tests
{
    public static class IdentityServerHostHelpers
    {
        public static DiscoveryClient GetDiscoveryClient(this IdentityServerHost identityServerHost)
            => new DiscoveryClient(identityServerHost.BaseAddress.ToString());

        public static IntrospectionClient GetIntroClient(this IdentityServerHost identityServerHost)
        {
            var discoveryClient = identityServerHost.GetDiscoveryClient();
            var doc = discoveryClient.GetAsync().Result;

            return new IntrospectionClient(doc.IntrospectionEndpoint, "identityserverhost", "testsecret");
        }

        public static TokenClient GetTokenClient(this IdentityServerHost identityServerHost)
        {
            var discoveryClient = identityServerHost.GetDiscoveryClient();
            var doc = discoveryClient.GetAsync().Result;

            return new TokenClient(doc.TokenEndpoint, "testclient", "clientsecret");
        }
    }
}
