// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Microsoft.Extensions.Configuration;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.IdentityServer.IdentityResources;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Security.Claims;
using Volo.Abp.Uow;

using X.Abp.IdentityServer;

using ApiResource = Volo.Abp.IdentityServer.ApiResources.ApiResource;
using ApiScope = Volo.Abp.IdentityServer.ApiScopes.ApiScope;
using Client = Volo.Abp.IdentityServer.Clients.Client;

namespace AbpVnext.Pro.IdentityServer;

public class IdentityServerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
#pragma warning disable CA1861 // 不要将常量数组作为参数
    protected IApiResourceRepository ApiResourceRepository { get; }

    protected IApiScopeRepository ApiScopeRepository { get; }

    protected IClientRepository ClientRepository { get; }

    protected IIdentityResourceDataSeeder IdentityResourceDataSeeder { get; }

    protected IGuidGenerator GuidGenerator { get; }

    protected IPermissionDataSeeder PermissionDataSeeder { get; }

    protected IConfiguration Configuration { get; }

    protected ICurrentTenant CurrentTenant { get; }

    public IdentityServerDataSeedContributor(
        IClientRepository clientRepository,
        IApiResourceRepository apiResourceRepository,
        IApiScopeRepository apiScopeRepository,
        IIdentityResourceDataSeeder identityResourceDataSeeder,
        IGuidGenerator guidGenerator,
        IPermissionDataSeeder permissionDataSeeder,
        IConfiguration configuration,
        ICurrentTenant currentTenant)
    {
        ClientRepository = clientRepository;
        ApiResourceRepository = apiResourceRepository;
        ApiScopeRepository = apiScopeRepository;
        IdentityResourceDataSeeder = identityResourceDataSeeder;
        GuidGenerator = guidGenerator;
        PermissionDataSeeder = permissionDataSeeder;
        Configuration = configuration;
        CurrentTenant = currentTenant;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        using (CurrentTenant.Change(context?.TenantId))
        {
            await IdentityResourceDataSeeder.CreateStandardResourcesAsync();
            await CreateApiResourcesAsync();
            await CreateApiScopesAsync();
            await CreateClientsAsync();
        }
    }

    private async Task CreateApiScopesAsync()
    {
        await CreateApiScopeAsync("Pro");
    }

    private async Task CreateApiResourcesAsync()
    {
        var commonApiUserClaims = new[]
        {
                "email",
                "email_verified",
                "name",
                "phone_number",
                "phone_number_verified",
                "role",
                AbpClaimTypes.SessionId,
                AbpClaimTypes.SurName,
                AbpClaimTypes.Name
        };

        await CreateApiResourceAsync("Pro", commonApiUserClaims);
        /*
        var configurationSection = Configuration.GetSection("IdentityServer:Clients");
        var consoleAndAngularClientId = configurationSection["Pro_App:ClientId"];
        if (!consoleAndAngularClientId.IsNullOrWhiteSpace())
        {
            await CreateApiResourceAsync(consoleAndAngularClientId, (configurationSection["Pro_App:ClientSecret"] ?? "1q2w3e*").Sha256(), commonApiUserClaims);
        }
        */
    }

    private async Task<ApiResource> CreateApiResourceAsync(string name, IEnumerable<string> claims)
    {
        var apiResource = await ApiResourceRepository.FindByNameAsync(name);
        if (apiResource == null)
        {
            apiResource = await ApiResourceRepository.InsertAsync(new ApiResource(GuidGenerator.Create(), name, name + " API"), true);
        }

        foreach (var claim in claims)
        {
            if (apiResource.FindClaim(claim) == null)
            {
                apiResource.AddUserClaim(claim);
            }
        }

        return await ApiResourceRepository.UpdateAsync(apiResource);
    }

    /*
    private async Task<ApiResource> CreateApiResourceAsync(string name, string secret, IEnumerable<string> claims)
    {
        var apiResource = await ApiResourceRepository.FindByNameAsync(name);
        apiResource ??= await ApiResourceRepository.InsertAsync(
                new ApiResource(
                    GuidGenerator.Create(),
                    name,
                    name + " API"),
                autoSave: true);

        if (apiResource.FindSecret(secret) == null)
        {
            apiResource.AddSecret(secret);
        }

        foreach (var claim in claims)
        {
            if (apiResource.FindClaim(claim) == null)
            {
                apiResource.AddUserClaim(claim);
            }
        }

        return await ApiResourceRepository.UpdateAsync(apiResource);
    }
    */
    private async Task<ApiScope> CreateApiScopeAsync(string name)
    {
        var apiScope = await ApiScopeRepository.FindByNameAsync(name);
        apiScope ??= await ApiScopeRepository.InsertAsync(
                new ApiScope(
                    GuidGenerator.Create(),
                    name,
                    name + " API"),
                autoSave: true);

        return apiScope;
    }

    private async Task CreateClientsAsync()
    {
        var commonScopes = new[]
        {
                "email",
                "openid",
                "profile",
                "role",
                "phone",
                "address",
                "Pro"
        };

        var configurationSection = Configuration.GetSection("IdentityServer:Clients");

        // Web Public Client
        var webPublicClientId = configurationSection["Pro_Web:ClientId"];
        if (!webPublicClientId.IsNullOrWhiteSpace())
        {
            var webPublicClientRootUrl = configurationSection["Pro_Web:RootUrl"].EnsureEndsWith('/');

            await CreateClientAsync(
                name: webPublicClientId,
                clientUri: webPublicClientRootUrl,
                logoUri: "/images/clients/aspnetcore.svg",
                scopes: commonScopes,
                grantTypes: new[] { "hybrid" },
                secret: (configurationSection["Pro_Web:ClientSecret"] ?? "1q2w3e*").Sha256(),
                redirectUri: $"{webPublicClientRootUrl}signin-oidc",
                postLogoutRedirectUri: $"{webPublicClientRootUrl}signout-callback-oidc",
                frontChannelLogoutUri: $"{webPublicClientRootUrl}Account/FrontChannelLogout");
        }

        // Console Test / Angular Client
        var consoleAndAngularClientId = configurationSection["Pro_App:ClientId"];
        if (!consoleAndAngularClientId.IsNullOrWhiteSpace())
        {
            var webClientRootUrl = configurationSection["Pro_App:RootUrl"]?.TrimEnd('/');

            await CreateClientAsync(
                name: consoleAndAngularClientId,
                clientUri: webClientRootUrl,
                scopes: commonScopes,
                logoUri: "/images/clients/angular.svg",
                grantTypes: new[] { "password", "client_credentials", "authorization_code", "LinkLogin", "Impersonation", "sms", "weixin_mini_program", "mca", "bind_third_party", "SpaExternalLogin" },
                secret: (configurationSection["Pro_App:ClientSecret"] ?? "1q2w3e*").Sha256(),
                requireClientSecret: false,
                redirectUri: webClientRootUrl,
                postLogoutRedirectUri: webClientRootUrl,
                corsOrigins: new[] { webClientRootUrl.RemovePostFix("/") });
        }

        // Blazor Client
        var blazorClientId = configurationSection["Pro_BlazorServerTiered:ClientId"];
        if (!blazorClientId.IsNullOrWhiteSpace())
        {
            var blazorRootUrl = configurationSection["Pro_BlazorServerTiered:RootUrl"].TrimEnd('/');

            await CreateClientAsync(
                name: blazorClientId,
                scopes: commonScopes,
                grantTypes: new[] { "authorization_code" },
                secret: configurationSection["Pro_BlazorServerTiered:ClientSecret"]?.Sha256(),
                requireClientSecret: false,
                redirectUri: $"{blazorRootUrl}/authentication/login-callback",
                postLogoutRedirectUri: $"{blazorRootUrl}/authentication/logout-callback");
        }

        // Swagger Client
        var swaggerClientId = configurationSection["Pro_Swagger:ClientId"];
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var swaggerRootUrl = configurationSection["Pro_Swagger:RootUrl"].TrimEnd('/');

            await CreateClientAsync(
                name: swaggerClientId,
                clientUri: swaggerRootUrl,
                logoUri: "/images/clients/swagger.svg",
                scopes: commonScopes,
                grantTypes: new[] { "authorization_code" },
                secret: (configurationSection["Pro_Swagger:ClientSecret"] ?? "1q2w3e*").Sha256(),
                requireClientSecret: false,
                redirectUri: $"{swaggerRootUrl}/swagger/oauth2-redirect.html",
                corsOrigins: new[] { swaggerRootUrl.RemovePostFix("/") });
        }
    }

    private async Task<Client> CreateClientAsync(
        string name,
        IEnumerable<string> scopes,
        IEnumerable<string> grantTypes,
        string clientUri = null,
        string secret = null,
        string redirectUri = null,
        string postLogoutRedirectUri = null,
        string frontChannelLogoutUri = null,
        bool requireClientSecret = true,
        bool requirePkce = false,
        IEnumerable<string> permissions = null,
        IEnumerable<string> corsOrigins = null,
        string logoUri = null)
    {
        var client = await ClientRepository.FindByClientIdAsync(name);
        client ??= await ClientRepository.InsertAsync(
                new Client(
                    GuidGenerator.Create(),
                    name)
                {
                    ClientName = name,
                    ClientUri = clientUri,
                    LogoUri = logoUri,
                    ProtocolType = "oidc",
                    Description = name,
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AllowOfflineAccess = true,
                    AbsoluteRefreshTokenLifetime = 31536000, // 365 days
                    AccessTokenLifetime = 31536000, // 365 days
                    AuthorizationCodeLifetime = 300,
                    IdentityTokenLifetime = 300,
                    RequireConsent = false,
                    FrontChannelLogoutUri = frontChannelLogoutUri,
                    RequireClientSecret = requireClientSecret,
                    RequirePkce = requirePkce
                },
                autoSave: true);

        if (client.ClientUri != clientUri)
        {
            client.ClientUri = clientUri;
        }

        if (client.LogoUri != logoUri)
        {
            client.LogoUri = logoUri;
        }

        foreach (var scope in scopes)
        {
            if (client.FindScope(scope) == null)
            {
                client.AddScope(scope);
            }
        }

        grantTypes.Validate();
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

        if (redirectUri != null)
        {
            if (client.FindRedirectUri(redirectUri) == null)
            {
                client.AddRedirectUri(redirectUri);
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
            await PermissionDataSeeder.SeedAsync(
                ClientPermissionValueProvider.ProviderName,
                name,
                permissions,
                null);
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

        return await ClientRepository.UpdateAsync(client);
    }
}
