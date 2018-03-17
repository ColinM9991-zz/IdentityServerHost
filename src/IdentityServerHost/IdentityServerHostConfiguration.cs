using IdentityServer4.Models;
using Microsoft.IdentityModel.Tokens;

namespace IdentityServerHost
{
    public class IdentityServerHostConfiguration
    {
        internal Client[] Clients { get; private set; } = { };

        internal ApiResource[] ApiResources { get; private set; } = { };

        internal IdentityResource[] IdentityResources { get; private set; } = { };

        internal SigningCredentials SigningCredentials { get; private set; }

        public IdentityServerHostConfiguration AddClients(params Client[] clients)
        {
            Clients = clients;
            return this;
        }

        public IdentityServerHostConfiguration AddIdentityResources(params IdentityResource[] identityResources)
        {
            IdentityResources = identityResources;
            return this;
        }

        public IdentityServerHostConfiguration AddApiResources(params ApiResource[] apiResources)
        {
            ApiResources = apiResources;
            return this;
        }

        public IdentityServerHostConfiguration AddSigningCredential(SigningCredentials credentials)
        {
            SigningCredentials = credentials;
            return this;
        }
    }
}
