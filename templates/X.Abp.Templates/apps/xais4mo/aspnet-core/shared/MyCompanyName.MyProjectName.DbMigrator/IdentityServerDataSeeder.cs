using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Microsoft.Extensions.Configuration;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

using X.Abp.Identity.Permissions;
using X.Abp.IdentityServer.ApiResources;
using X.Abp.IdentityServer.ApiScopes;
using X.Abp.IdentityServer.Clients;
using X.Abp.IdentityServer.IdentityResources;

using ApiResource = X.Abp.IdentityServer.ApiResources.ApiResource;
using ApiScope = X.Abp.IdentityServer.ApiScopes.ApiScope;
using Client = X.Abp.IdentityServer.Clients.Client;

namespace MyCompanyName.MyProjectName.DbMigrator;

public class IdentityServerDataSeeder : ITransientDependency
{
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IApiScopeRepository _apiScopeRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IIdentityResourceDataSeeder _identityResourceDataSeeder;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;

    public IdentityServerDataSeeder(
        IClientRepository clientRepository,
        IApiResourceRepository apiResourceRepository,
        IApiScopeRepository apiScopeRepository,
        IIdentityResourceDataSeeder identityResourceDataSeeder,
        IGuidGenerator guidGenerator,
        IPermissionDataSeeder permissionDataSeeder,
        IConfiguration configuration,
        ICurrentTenant currentTenant)
    {
        _clientRepository = clientRepository;
        _apiResourceRepository = apiResourceRepository;
        _apiScopeRepository = apiScopeRepository;
        _identityResourceDataSeeder = identityResourceDataSeeder;
        _guidGenerator = guidGenerator;
        _permissionDataSeeder = permissionDataSeeder;
        _configuration = configuration;
        _currentTenant = currentTenant;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync()
    {
        using (_currentTenant.Change(null))
        {
            await _identityResourceDataSeeder.CreateStandardResourcesAsync();
            await CreateApiResourcesAsync();
            await CreateApiScopesAsync();
            await CreateWebGatewaySwaggerClientsAsync();
            await CreateClientsAsync();
        }
    }

    private async Task CreateApiResourcesAsync()
    {
        var commonApiUserClaims =
            new[] { "email", "email_verified", "name", "phone_number", "phone_number_verified", "role" };

        await CreateApiResourceAsync("AccountService", commonApiUserClaims);
        await CreateApiResourceAsync("IdentityService", commonApiUserClaims);
        await CreateApiResourceAsync("AdministrationService", commonApiUserClaims);
        await CreateApiResourceAsync("SaasService", commonApiUserClaims);
        await CreateApiResourceAsync("ProductService", commonApiUserClaims);
    }

    private async Task CreateApiScopesAsync()
    {
        await CreateApiScopeAsync("AccountService");
        await CreateApiScopeAsync("IdentityService");
        await CreateApiScopeAsync("AdministrationService");
        await CreateApiScopeAsync("SaasService");
        await CreateApiScopeAsync("ProductService");
    }

    private async Task CreateWebGatewaySwaggerClientsAsync()
    {
        await CreateSwaggerClientAsync("WebGateway", new[] { "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService" });
    }

    private async Task CreateSwaggerClientAsync(string name, string[] scopes = null)
    {
        var commonScopes = new[] { "email", "openid", "profile", "role", "phone", "address" };
        scopes ??= new[] { name };

        // Swagger Client
        var swaggerClientId = $"{name}_Swagger";
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var webGatewaySwaggerRootUrl = _configuration[$"IdentityServer:Clients:{name}:RootUrl"].TrimEnd('/');
            var publicWebGatewayRootUrl = _configuration[$"IdentityServer:Clients:PublicWebGateway:RootUrl"].TrimEnd('/');
            var accountServiceRootUrl = _configuration[$"IdentityServer:Resources:AccountService:RootUrl"].TrimEnd('/');
            var identityServiceRootUrl = _configuration[$"IdentityServer:Resources:IdentityService:RootUrl"].TrimEnd('/');
            var administrationServiceRootUrl = _configuration[$"IdentityServer:Resources:AdministrationService:RootUrl"].TrimEnd('/');
            var saasServiceRootUrl = _configuration[$"IdentityServer:Resources:SaasService:RootUrl"].TrimEnd('/');
            var productServiceRootUrl = _configuration[$"IdentityServer:Resources:ProductService:RootUrl"].TrimEnd('/');

            await CreateClientAsync(
                name: swaggerClientId,
                scopes: commonScopes.Union(scopes),
                grantTypes: new[] { "authorization_code" },
                secret: "1q2w3e*".Sha256(),
                requireClientSecret: false,
                redirectUris: new List<string> {
                    $"{webGatewaySwaggerRootUrl}/swagger/oauth2-redirect.html", // WebGateway redirect uri
                    $"{publicWebGatewayRootUrl}/swagger/oauth2-redirect.html", // PublicWebGateway redirect uri
                    $"{accountServiceRootUrl}/swagger/oauth2-redirect.html", // AccountService redirect uri
                    $"{identityServiceRootUrl}/swagger/oauth2-redirect.html", // IdentityService redirect uri
                    $"{administrationServiceRootUrl}/swagger/oauth2-redirect.html", // AdministrationService redirect uri
                    $"{saasServiceRootUrl}/swagger/oauth2-redirect.html", // SaasService redirect uri
                    $"{productServiceRootUrl}/swagger/oauth2-redirect.html", // ProductService redirect uri
                },
                corsOrigins: new[] {
                    webGatewaySwaggerRootUrl.RemovePostFix("/"),
                    publicWebGatewayRootUrl.RemovePostFix("/"),
                    accountServiceRootUrl.RemovePostFix("/"),
                    identityServiceRootUrl.RemovePostFix("/"),
                    administrationServiceRootUrl.RemovePostFix("/"),
                    saasServiceRootUrl.RemovePostFix("/"),
                    productServiceRootUrl.RemovePostFix("/")
                }
            );
        }
    }

    private async Task<ApiResource> CreateApiResourceAsync(string name, IEnumerable<string> claims)
    {
        var apiResource = await _apiResourceRepository.FindByNameAsync(name);
        if (apiResource == null)
        {
            apiResource = await _apiResourceRepository.InsertAsync(
                new ApiResource(
                    _guidGenerator.Create(),
                    name,
                    name + " API"
                ),
                autoSave: true
            );
        }

        foreach (var claim in claims)
        {
            if (apiResource.FindClaim(claim) == null)
            {
                apiResource.AddUserClaim(claim);
            }
        }

        return await _apiResourceRepository.UpdateAsync(apiResource);
    }

    private async Task<ApiScope> CreateApiScopeAsync(string name)
    {
        var apiScope = await _apiScopeRepository.FindByNameAsync(name);
        if (apiScope == null)
        {
            apiScope = await _apiScopeRepository.InsertAsync(
                new ApiScope(
                    _guidGenerator.Create(),
                    name,
                    name + " API"
                ),
                autoSave: true
            );
        }

        return apiScope;
    }

    private async Task CreateClientsAsync()
    {
        var commonScopes = new[] { "email", "openid", "profile", "role", "phone", "address" };

        //Web Client
        var webClientRootUrl = _configuration["IdentityServer:Clients:MyProjectName_Web:RootUrl"].EnsureEndsWith('/');
        await CreateClientAsync(
            name: "MyProjectName_Web",
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }),
            grantTypes: new[] { "hybrid" },
            secret: "1q2w3e*".Sha256(),
            redirectUris: new List<string> { $"{webClientRootUrl}signin-oidc" },
            postLogoutRedirectUri: $"{webClientRootUrl}signout-callback-oidc",
            frontChannelLogoutUri: $"{webClientRootUrl}Account/FrontChannelLogout",
            corsOrigins: new[] { webClientRootUrl.RemovePostFix("/") }
        );

        //Blazor Client
        var blazorClientRootUrl = _configuration["IdentityServer:Clients:MyProjectName_Blazor:RootUrl"].EnsureEndsWith('/');
        await CreateClientAsync(
            name: "MyProjectName_Blazor",
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }),
            grantTypes: new[] { "authorization_code" },
            secret: "1q2w3e*".Sha256(),
            requireClientSecret: false,
            redirectUris: new List<string> { $"{blazorClientRootUrl}authentication/login-callback" },
            postLogoutRedirectUri: $"{blazorClientRootUrl}authentication/logout-callback",
            corsOrigins: new[] { blazorClientRootUrl.RemovePostFix("/") }
        );

        //Blazor Server Client
        var blazorServerClientRootUrl = _configuration["IdentityServer:Clients:MyProjectName_BlazorServer:RootUrl"].EnsureEndsWith('/');
        await CreateClientAsync(
            name: "MyProjectName_BlazorServer",
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }),
            grantTypes: new[] { "hybrid" },
            secret: "1q2w3e*".Sha256(),
            redirectUris: new List<string> { $"{blazorServerClientRootUrl}signin-oidc" },
            postLogoutRedirectUri: $"{blazorServerClientRootUrl}signout-callback-oidc",
            frontChannelLogoutUri: $"{blazorServerClientRootUrl}Account/FrontChannelLogout",
            corsOrigins: new[] { blazorServerClientRootUrl.RemovePostFix("/") }
        );

        //Public Web Client
        var publicWebClientRootUrl = _configuration["IdentityServer:Clients:MyProjectName_PublicWeb:RootUrl"]
            .EnsureEndsWith('/');
        await CreateClientAsync(
            name: "MyProjectName_PublicWeb",
            scopes: commonScopes.Union(new[] { "AccountService", "AdministrationService", "ProductService" }),
            grantTypes: new[] { "hybrid" },
            secret: "1q2w3e*".Sha256(),
            redirectUris: new List<string> { $"{publicWebClientRootUrl}signin-oidc" },
            postLogoutRedirectUri: $"{publicWebClientRootUrl}signout-callback-oidc",
            frontChannelLogoutUri: $"{publicWebClientRootUrl}Account/FrontChannelLogout",
            corsOrigins: new[] { publicWebClientRootUrl.RemovePostFix("/") }
        );

        //Angular Client
        var angularClientRootUrl = _configuration["IdentityServer:Clients:MyProjectName_Angular:RootUrl"].TrimEnd('/');
        await CreateClientAsync(
            name: "MyProjectName_Angular",
            scopes: commonScopes.Union(new[] {
                "AccountService", "IdentityService", "AdministrationService", "SaasService", "ProductService"
            }),
            grantTypes: new[] { "authorization_code", "LinkLogin", "Impersonation" },
            secret: "1q2w3e*".Sha256(),
            requireClientSecret: false,
            redirectUris: new List<string> { $"{angularClientRootUrl}" },
            postLogoutRedirectUri: $"{angularClientRootUrl}",
            corsOrigins: new[] { angularClientRootUrl }
        );

        //Administration Service Client
        await CreateClientAsync(
            name: "MyProjectName_AdministrationService",
            scopes: commonScopes.Union(new[] { "IdentityService" }),
            grantTypes: new[] { "client_credentials" },
            secret: "1q2w3e*".Sha256(),
            permissions: new[] { AbpIdentityProPermissions.Users.Default }
        );
    }

    private async Task<Client> CreateClientAsync(
        string name,
        IEnumerable<string> scopes,
        IEnumerable<string> grantTypes,
        string secret = null,
        List<string> redirectUris = null,
        string postLogoutRedirectUri = null,
        string frontChannelLogoutUri = null,
        bool requireClientSecret = true,
        bool requirePkce = false,
        IEnumerable<string> permissions = null,
        IEnumerable<string> corsOrigins = null)
    {
        var client = await _clientRepository.FindByClientIdAsync(name);
        if (client == null)
        {
            client = await _clientRepository.InsertAsync(
                new Client(
                    _guidGenerator.Create(),
                    name
                ) {
                    ClientName = name,
                    ProtocolType = "oidc",
                    Description = name,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    AbsoluteRefreshTokenLifetime = 31536000, //365 days
                    AccessTokenLifetime = 31536000, //365 days
                    AuthorizationCodeLifetime = 300,
                    IdentityTokenLifetime = 300,
                    RequireConsent = false,
                    FrontChannelLogoutUri = frontChannelLogoutUri,
                    RequireClientSecret = requireClientSecret,
                    RequirePkce = requirePkce
                },
                autoSave: true
            );
        }

        foreach (var scope in scopes)
        {
            if (client.FindScope(scope) == null)
            {
                client.AddScope(scope);
            }
        }

        Client.ValidateGrantTypes(grantTypes);
        foreach (var grantType in grantTypes)
        {
            if (client.FindGrantType(grantType) == null)
            {
                client.AddGrantType(grantType);
            }
        }

        if (!secret.IsNullOrEmpty())
        {
            if (client.FindSecret(secret) == null)
            {
                client.AddSecret(secret);
            }
        }

        if (redirectUris != null)
        {
            foreach (var redirectUri in redirectUris)
            {
                if (redirectUri == null)
                {
                    continue;
                }

                if (client.FindRedirectUri(redirectUri) == null)
                {
                    client.AddRedirectUri(redirectUri);
                }
            }
        }

        if (postLogoutRedirectUri != null)
        {
            if (client.FindPostLogoutRedirectUri(postLogoutRedirectUri) == null)
            {
                client.AddPostLogoutRedirectUri(postLogoutRedirectUri);
            }
        }

        if (permissions != null)
        {
            await _permissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                name,
                permissions,
                null
            );
        }

        if (corsOrigins != null)
        {
            foreach (var origin in corsOrigins)
            {
                if (!origin.IsNullOrWhiteSpace() && client.FindCorsOrigin(origin) == null)
                {
                    client.AddCorsOrigin(origin);
                }
            }
        }

        return await _clientRepository.UpdateAsync(client);
    }
}
