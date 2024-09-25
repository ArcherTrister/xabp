// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityServer4.Models;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.ObjectExtending;

using X.Abp.IdentityServer.Client.Dtos;
using X.Abp.IdentityServer.Permissions;

namespace X.Abp.IdentityServer.Client;

[Authorize(AbpIdentityServerProPermissions.Client.Default)]
public class ClientAppService : IdentityServerAppServiceBase, IClientAppService
{
    protected IClientRepository ClientRepository { get; }

    public ClientAppService(IClientRepository clientRepository)
    {
        ClientRepository = clientRepository;
    }

    public virtual async Task<PagedResultDto<ClientWithDetailsDto>> GetListAsync(GetClientListInput input)
    {
        var clients = await ClientRepository.GetListAsync(input.Sorting, input.SkipCount, input.MaxResultCount);
        var count = await ClientRepository.GetCountAsync();
        var list = ObjectMapper.Map<List<Volo.Abp.IdentityServer.Clients.Client>, List<ClientWithDetailsDto>>(clients);
        return new PagedResultDto<ClientWithDetailsDto>(count, list);
    }

    public virtual async Task<ClientWithDetailsDto> GetAsync(Guid id)
    {
        var client = await ClientRepository.GetAsync(id);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.Clients.Client, ClientWithDetailsDto>(client);
    }

    [Authorize(AbpIdentityServerProPermissions.Client.Create)]
    public virtual async Task<ClientWithDetailsDto> CreateAsync(CreateClientDto input)
    {
        await CheckClientIdExistAsync(input.ClientId);
        var client = new Volo.Abp.IdentityServer.Clients.Client(GuidGenerator.Create(), input.ClientId)
        {
            ClientName = input.ClientName,
            Description = input.Description,
            ClientUri = input.ClientUri,
            LogoUri = input.LogoUri,
            RequireConsent = input.RequireConsent
        };
        foreach (var clientSecretDto in input.Secrets)
        {
            clientSecretDto.Value = clientSecretDto.Value.Sha256();
            client.AddSecret(clientSecretDto.Value, clientSecretDto.Expiration, clientSecretDto.Type, clientSecretDto.Description);
        }

        var scopes = input.Scopes;
        for (var i = 0; i < scopes.Length; i++)
        {
            client.AddScope(scopes[i]);
        }

        input.MapExtraPropertiesTo(client);
        var result = await ClientRepository.InsertAsync(client);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.Clients.Client, ClientWithDetailsDto>(result);
    }

    [Authorize(AbpIdentityServerProPermissions.Client.Update)]
    public virtual async Task<ClientWithDetailsDto> UpdateAsync(Guid id, UpdateClientDto input)
    {
        var client = await ClientRepository.GetAsync(id);
        client.ClientName = input.ClientName;
        client.Description = input.Description;
        client.ClientUri = input.ClientUri;
        client.LogoUri = input.LogoUri;
        client.Enabled = input.Enabled;
        client.RequireConsent = input.RequireConsent;
        client.AccessTokenLifetime = input.AccessTokenLifetime;
        client.ConsentLifetime = input.ConsentLifetime;
        client.AccessTokenType = input.AccessTokenType;
        client.EnableLocalLogin = input.EnableLocalLogin;
        client.IncludeJwtId = input.IncludeJwtId;
        client.AllowOfflineAccess = input.AllowOfflineAccess;
        client.AlwaysSendClientClaims = input.AlwaysSendClientClaims;
        client.PairWiseSubjectSalt = input.PairWiseSubjectSalt;
        client.UserSsoLifetime = input.UserSsoLifetime;
        client.UserCodeType = input.UserCodeType;
        client.DeviceCodeLifetime = input.DeviceCodeLifetime;
        client.AllowRememberConsent = input.AllowRememberConsent;
        client.RequirePkce = input.RequirePkce;
        client.RequireClientSecret = input.RequireClientSecret;
        UpdateClientSecrets(input, client);
        UpdateClientClaims(input, client);
        UpdateClientProperties(input, client);
        UpdateClientRedirectUris(input, client);
        UpdateClientPostLogoutRedirectUris(input, client);
        UpdateClientCorsOrigins(input, client);
        UpdateClientGrantTypes(input, client);
        UpdateClientIdentityProviderRestrictions(input, client);
        UpdateClientScopes(input, client);
        input.MapExtraPropertiesTo(client);
        var result = await ClientRepository.UpdateAsync(client);
        return ObjectMapper.Map<Volo.Abp.IdentityServer.Clients.Client, ClientWithDetailsDto>(result);
    }

    [Authorize(AbpIdentityServerProPermissions.Client.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        await ClientRepository.DeleteAsync(id);
    }

    protected virtual void UpdateClientSecrets(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        foreach (var clientSecretDto in input.Secrets)
        {
            if (client.FindSecret(clientSecretDto.Value, clientSecretDto.Type) == null)
            {
                clientSecretDto.Value = clientSecretDto.Value.Sha256();
                client.AddSecret(clientSecretDto.Value, clientSecretDto.Expiration, clientSecretDto.Type, clientSecretDto.Description);
            }
        }

        using var enumerator = client.ClientSecrets.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var clientSecret = enumerator.Current;
            if (input.Secrets.FirstOrDefault((s) => clientSecret.Equals(client.Id, s.Value, s.Type)) == null)
            {
                client.RemoveSecret(clientSecret.Value, clientSecret.Type);
            }
        }
    }

    protected virtual void UpdateClientProperties(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        foreach (var clientPropertyDto in input.Properties)
        {
            if (client.FindProperty(clientPropertyDto.Key) == null)
            {
                client.AddProperty(clientPropertyDto.Key, clientPropertyDto.Value);
            }
        }

        using var enumerator = client.Properties.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var property = enumerator.Current;
            if (input.Properties.FirstOrDefault((p) => property.Equals(client.Id, p.Key, p.Value)) == null)
            {
                client.RemoveProperty(property.Key);
            }
        }
    }

    protected virtual void UpdateClientClaims(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        foreach (var clientClaimDto in input.Claims)
        {
            if (client.FindClaim(clientClaimDto.Type, clientClaimDto.Value) == null)
            {
                client.AddClaim(clientClaimDto.Type, clientClaimDto.Value);
            }
        }

        using var enumerator = client.Claims.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var claim = enumerator.Current;
            if (input.Claims.FirstOrDefault((c) => claim.Equals(client.Id, c.Type, c.Value)) == null)
            {
                client.RemoveClaim(claim.Type, claim.Value);
            }
        }
    }

    protected virtual void UpdateClientRedirectUris(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        foreach (var text in input.RedirectUris)
        {
            if (client.FindRedirectUri(text) == null)
            {
                client.AddRedirectUri(text);
            }
        }

        using var enumerator = client.RedirectUris.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var redirectUri = enumerator.Current;
            if (!input.RedirectUris.Any((uri) => redirectUri.Equals(client.Id, uri)))
            {
                client.RemoveRedirectUri(redirectUri.RedirectUri);
            }
        }
    }

    protected virtual void UpdateClientCorsOrigins(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        foreach (var text in input.AllowedCorsOrigins)
        {
            if (client.FindCorsOrigin(text) == null)
            {
                client.AddCorsOrigin(text);
            }
        }

        using var enumerator = client.AllowedCorsOrigins.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var corsOrigin = enumerator.Current;
            if (!input.AllowedCorsOrigins.Any((uri) => corsOrigin.Equals(client.Id, uri)))
            {
                client.RemoveCorsOrigin(corsOrigin.Origin);
            }
        }
    }

    protected virtual void UpdateClientPostLogoutRedirectUris(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        foreach (var text in input.PostLogoutRedirectUris)
        {
            if (client.FindPostLogoutRedirectUri(text) == null)
            {
                client.AddPostLogoutRedirectUri(text);
            }
        }

        using var enumerator = client.PostLogoutRedirectUris.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var postLogoutRedirectUri = enumerator.Current;
            if (!input.PostLogoutRedirectUris.Any((uri) => postLogoutRedirectUri.Equals(client.Id, uri)))
            {
                client.RemovePostLogoutRedirectUri(postLogoutRedirectUri.PostLogoutRedirectUri);
            }
        }
    }

    protected virtual void UpdateClientIdentityProviderRestrictions(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        foreach (var text in input.IdentityProviderRestrictions)
        {
            if (client.FindIdentityProviderRestriction(text) == null)
            {
                client.AddIdentityProviderRestriction(text);
            }
        }

        using var enumerator = client.IdentityProviderRestrictions.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var idPRestriction = enumerator.Current;
            if (!input.IdentityProviderRestrictions.Any((p) => idPRestriction.Equals(client.Id, p)))
            {
                client.RemoveIdentityProviderRestriction(idPRestriction.Provider);
            }
        }
    }

    protected virtual void UpdateClientGrantTypes(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        input.AllowedGrantTypes.Validate();
        foreach (var text in input.AllowedGrantTypes)
        {
            if (client.FindGrantType(text) == null)
            {
                client.AddGrantType(text);
            }
        }

        using var enumerator = client.AllowedGrantTypes.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var grantType = enumerator.Current;
            if (!input.AllowedGrantTypes.Any((g) => grantType.Equals(client.Id, g)))
            {
                client.RemoveGrantType(grantType.GrantType);
            }
        }
    }

    protected virtual void UpdateClientScopes(UpdateClientDto input, Volo.Abp.IdentityServer.Clients.Client client)
    {
        foreach (var scope in input.AllowedScopes)
        {
            if (client.FindScope(scope) == null)
            {
                client.AddScope(scope);
            }
        }

        using var enumerator = client.AllowedScopes.ToList().GetEnumerator();
        while (enumerator.MoveNext())
        {
            var scope = enumerator.Current;
            if (!input.AllowedScopes.Any((s) => scope.Equals(client.Id, s)))
            {
                client.RemoveScope(scope.Scope);
            }
        }
    }

    protected virtual async Task CheckClientIdExistAsync(string clientId)
    {
        var flag = await ClientRepository.CheckClientIdExistAsync(clientId);
        if (flag)
        {
            throw new BusinessException("Volo.IdentityServer:DuplicateClientId").WithData("ClientId", clientId);
        }
    }
}
