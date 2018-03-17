# IdentityServerHost

## Description

This package provides an implementation of `IWebHost` for Identity Server, and was inspired by the [IdentityServer4.Mock](https://github.com/mtranter/IdentityServer4.Mock) package which uses `TestServer` and injects Identity Server services & middleware.

The problem with the above package is that because `TestServer` doesn't use sockets then it can't actually be used to create a mock identity server and have your production application communicate with that when writing integration tests.

To fix the issue of testing authorization in your production app at build time, this package uses Kestrel to host the Identity Server mock which allows for communication between applications hosted with `TestServer`.

Please beware, that because Kestrel doesn't have port sharing then it is recommended to instantiate and start the `IdentityServerHost` in a one-time initialize method, or a static constructor, in your test class or base class, this also prevents an endless amount of hosts being started for each of your tests.

Also please take note of the following [issue](https://github.com/aspnet/KestrelHttpServer/issues/1292).

## Usage

```csharp
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
}
```