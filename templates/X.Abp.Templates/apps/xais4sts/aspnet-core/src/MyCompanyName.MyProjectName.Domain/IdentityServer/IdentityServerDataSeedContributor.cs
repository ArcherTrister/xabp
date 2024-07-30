// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Microsoft.Extensions.Configuration;

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Uow;

using X.Abp.IdentityServer.ApiResources;
using X.Abp.IdentityServer.ApiScopes;
using X.Abp.IdentityServer.Clients;
using X.Abp.IdentityServer.IdentityResources;

using ApiResource = X.Abp.IdentityServer.ApiResources.ApiResource;
using ApiScope = X.Abp.IdentityServer.ApiScopes.ApiScope;
using Client = X.Abp.IdentityServer.Clients.Client;

namespace MyCompanyName.MyProjectName.IdentityServer;

public class IdentityServerDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IApiResourceRepository _apiResourceRepository;
    private readonly IApiScopeRepository _apiScopeRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IIdentityResourceDataSeeder _identityResourceDataSeeder;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IPermissionDataSeeder _permissionDataSeeder;
    private readonly IConfiguration _configuration;
    private readonly ICurrentTenant _currentTenant;

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
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            await _identityResourceDataSeeder.CreateStandardResourcesAsync();
            await CreateApiResourcesAsync();
            await CreateApiScopesAsync();
            await CreateClientsAsync();
        }
    }

    private async Task CreateApiScopesAsync()
    {
        await CreateApiScopeAsync("MyProjectName");
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
                "role"
            };

        var configurationSection = _configuration.GetSection("IdentityServer:Clients");
        if (configurationSection.Value == null) { return; }
        await CreateApiResourceAsync(configurationSection["MyProjectName_App:ClientId"], (configurationSection["MyProjectName_App:ClientSecret"] ?? "1q2w3e*").Sha256(), commonApiUserClaims);
    }

    private async Task<ApiResource> CreateApiResourceAsync(string name, string secret, IEnumerable<string> claims)
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
        var commonScopes = new[]
        {
                "email",
                "openid",
                "profile",
                "role",
                "phone",
                "address",
                "MyProjectName"
            };

        var configurationSection = _configuration.GetSection("IdentityServer:Clients");

        //Web Public Client
        var webPublicClientId = configurationSection["MyProjectName_Web:ClientId"];
        if (!webPublicClientId.IsNullOrWhiteSpace())
        {
            var webPublicClientRootUrl = configurationSection["MyProjectName_Web:RootUrl"].EnsureEndsWith('/');

            await CreateClientAsync(
                name: webPublicClientId,
                clientUri: webPublicClientRootUrl,
                logoUri: "/images/clients/aspnetcore.svg",
                scopes: commonScopes,
                grantTypes: new[] { "hybrid" },
                secret: (configurationSection["MyProjectName_Web:ClientSecret"] ?? "1q2w3e*").Sha256(),
                redirectUri: $"{webPublicClientRootUrl}signin-oidc",
                postLogoutRedirectUri: $"{webPublicClientRootUrl}signout-callback-oidc",
                frontChannelLogoutUri: $"{webPublicClientRootUrl}Account/FrontChannelLogout"
            );
        }

        //Console Test / Angular Client
        var consoleAndAngularClientId = configurationSection["MyProjectName_App:ClientId"];
        if (!consoleAndAngularClientId.IsNullOrWhiteSpace())
        {
            var webClientRootUrl = configurationSection["MyProjectName_App:RootUrl"]?.TrimEnd('/');

            await CreateClientAsync(
                name: consoleAndAngularClientId,
                clientUri: webClientRootUrl,
                scopes: commonScopes,
                logoUri: "/images/clients/angular.svg",
                grantTypes: new[] { "password", "client_credentials", "authorization_code", "LinkLogin", "Impersonation", "sms", "wx_mini_program", "mca", "SpaExternalLogin" },
                secret: (configurationSection["MyProjectName_App:ClientSecret"] ?? "1q2w3e*").Sha256(),
                requireClientSecret: false,
                redirectUri: webClientRootUrl,
                postLogoutRedirectUri: webClientRootUrl,
                corsOrigins: new[] { webClientRootUrl.RemovePostFix("/") }
            );
        }

        // Blazor Client
        var blazorClientId = configurationSection["MyProjectName_BlazorServerTiered:ClientId"];
        if (!blazorClientId.IsNullOrWhiteSpace())
        {
            var blazorRootUrl = configurationSection["MyProjectName_BlazorServerTiered:RootUrl"].TrimEnd('/');

            await CreateClientAsync(
                name: blazorClientId,
                scopes: commonScopes,
                grantTypes: new[] { "authorization_code" },
                secret: configurationSection["MyProjectName_BlazorServerTiered:ClientSecret"]?.Sha256(),
                requireClientSecret: false,
                redirectUri: $"{blazorRootUrl}/authentication/login-callback",
                postLogoutRedirectUri: $"{blazorRootUrl}/authentication/logout-callback"
            );
        }

        // Swagger Client
        var swaggerClientId = configurationSection["MyProjectName_Swagger:ClientId"];
        if (!swaggerClientId.IsNullOrWhiteSpace())
        {
            var swaggerRootUrl = configurationSection["MyProjectName_Swagger:RootUrl"].TrimEnd('/');

            await CreateClientAsync(
                name: swaggerClientId,
                clientUri: swaggerRootUrl,
                logoUri: "/images/clients/swagger.svg",
                scopes: commonScopes,
                grantTypes: new[] { "authorization_code" },
                secret: (configurationSection["MyProjectName_Swagger:ClientSecret"] ?? "1q2w3e*").Sha256(),
                requireClientSecret: false,
                redirectUri: $"{swaggerRootUrl}/swagger/oauth2-redirect.html",
                corsOrigins: new[] { swaggerRootUrl.RemovePostFix("/") }
            );
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
        var client = await _clientRepository.FindByClientIdAsync(name);
        if (client == null)
        {
            client = await _clientRepository.InsertAsync(
                new Client(
                    _guidGenerator.Create(),
                    name
                )
                {
                    ClientName = name,
                    ClientUri = clientUri,
                    LogoUri = logoUri,
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
