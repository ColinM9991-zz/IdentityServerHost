using System;
using System.Collections.Generic;
using System.Net;
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IdentityServerHost.Tests
{
    [TestClass]
    public class IdentityServerHostTests
    {
        private static readonly IdentityServerHost identityServerHost;

        static IdentityServerHostTests()
        {
            var apiResources = new[]
            {
                new ApiResource("identityserverhost")
                {
                    Scopes = new List<Scope> {new Scope("identityserverhost")},
                    ApiSecrets = new List<Secret> {new Secret("testsecret".Sha256())}
                }
            };

            var clients = new[]
            {
                new Client
                {
                    ClientId = "testclient",
                    ClientSecrets = new[] {new Secret("clientsecret".Sha256())},
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new[] {"identityserverhost"}
                }
            };

            var configuration = new IdentityServerHostConfiguration().AddApiResources(apiResources).AddClients(clients);

            identityServerHost = new IdentityServerHost(configuration);
            identityServerHost.Start();
        }

        [TestMethod]
        public void
            ConstructingIdentityServerHost_WithNullConfiguration_ThrowsArgumentNullExceptionWithCorrectParameterName()
        {
            // Arrange
            const string expectedParameterName = "configuration";
            const IdentityServerHostConfiguration configuration = null;

            // Act
            void ConstructIdentityServerHost() => new IdentityServerHost(configuration);

            // Assert
            var exception = Assert.ThrowsException<ArgumentNullException>((Action) ConstructIdentityServerHost);
            Assert.AreEqual(expectedParameterName, exception.ParamName);
        }

        [TestMethod]
        public void GettingDiscoveryClient_WithValidConfiguration_ProvidesValidDiscoveryEndpoint()
        {
            // Arrange
            var discoveryClient = identityServerHost.GetDiscoveryClient();

            // Act

            // Assert
            Assert.IsNotNull(discoveryClient);
            Assert.IsNotNull(discoveryClient.Authority);
        }

        [TestMethod]
        public void GettingDiscoveryClientResponse_WithValidConfiguration_ProvidesValidDiscoveryAuthority()
        {
            // Arrange
            var discoveryClient = identityServerHost.GetDiscoveryClient();

            // Act
            var doc = discoveryClient.GetAsync().Result;

            // Assert
            Assert.IsNotNull(doc);
            Assert.IsNotNull(doc.TokenEndpoint);
            Assert.IsNotNull(doc.AuthorizeEndpoint);
            Assert.IsNotNull(doc.IntrospectionEndpoint);
        }

        [TestMethod]
        public void RequestingClientCredentials_WithValidConfiguration_GeneratesValidToken()
        {
            // Arrange
            var tokenClient = identityServerHost.GetTokenClient();

            // Act
            var clientCredential = tokenClient.RequestClientCredentialsAsync("identityserverhost").Result;

            // Assert
            Assert.IsNotNull(clientCredential.AccessToken);
        }

        [TestMethod]
        public void ValidatingClientToken_WithValidCredential_ReturnsOkStatusCode()
        {
            // Arrange
            var tokenClient = identityServerHost.GetTokenClient();
            var introClient = identityServerHost.GetIntroClient();

            var clientCredential = tokenClient.RequestClientCredentialsAsync("identityserverhost").Result;
            var introRequest = new IntrospectionRequest {ClientId = "testclient", Token = clientCredential.AccessToken};

            // Act
            var introResponse = introClient.SendAsync(introRequest).Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, introResponse.HttpStatusCode);
        }
    }
}