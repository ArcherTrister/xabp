// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using OpenIddict.Abstractions;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.ObjectExtending;
using Volo.Abp.OpenIddict.Applications;

using X.Abp.OpenIddict.Applications.Dtos;
using X.Abp.OpenIddict.Permissions;

namespace X.Abp.OpenIddict.Applications;

[Authorize(AbpOpenIddictProPermissions.Application.Default)]
public class ApplicationAppService : OpenIddictProAppServiceBase, IApplicationAppService
{
    protected IOpenIddictApplicationManager ApplicationManager { get; }

    protected IOpenIddictApplicationRepository ApplicationRepository { get; }

    public ApplicationAppService(IOpenIddictApplicationManager applicationManager, IOpenIddictApplicationRepository applicationRepository)
    {
        ApplicationManager = applicationManager;
        ApplicationRepository = applicationRepository;
    }

    public virtual async Task<ApplicationDto> GetAsync(Guid id)
    {
        var application = (await ApplicationManager.FindByIdAsync(ConvertIdentifierToString(id))) ?? throw new EntityNotFoundException(typeof(OpenIddictApplicationModel), id);
        return ObjectMapper.Map<OpenIddictApplicationModel, ApplicationDto>(application.As<OpenIddictApplicationModel>());
    }

    public virtual async Task<PagedResultDto<ApplicationDto>> GetListAsync(GetApplicationListInput input)
    {
        var apps = await ApplicationRepository.GetListAsync(input.Sorting, input.SkipCount, input.MaxResultCount, input.Filter);
        var num = await ApplicationRepository.GetCountAsync(input.Filter);
        var list = ObjectMapper.Map<List<OpenIddictApplicationModel>, List<ApplicationDto>>(apps.Select((x) => x.ToModel()).ToList());
        return new PagedResultDto<ApplicationDto>(num, list);
    }

    [Authorize(AbpOpenIddictProPermissions.Application.Create)]
    public virtual async Task<ApplicationDto> CreateAsync(CreateApplicationInput input)
    {
        await CheckInputDtoAsync(input);
        AbpApplicationDescriptor applicationDescriptor = new()
        {
            ApplicationType = input.ApplicationType,
            ClientId = input.ClientId,
            ClientSecret = input.ClientSecret,
            ConsentType = input.ConsentType,
            DisplayName = input.DisplayName,
            ClientType = input.ClientType,
            ClientUri = input.ClientUri,
            LogoUri = input.LogoUri
        };

        if (input.AllowHybridFlow)
        {
            input.AllowAuthorizationCodeFlow = true;
            input.AllowImplicitFlow = true;
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);
            if (string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
            }
        }

        if (input.AllowLogoutEndpoint)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);
        }

        if (input.AllowDeviceEndpoint)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
        }

        if (input.AllowAuthorizationCodeFlow)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
        }

        if (input.AllowClientCredentialsFlow)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
        }

        if (input.AllowImplicitFlow)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
        }

        if (input.AllowPasswordFlow)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
        }

        if (input.AllowRefreshTokenFlow)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
        }

        if (input.AllowAuthorizationCodeFlow || input.AllowImplicitFlow)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
        }

        if (input.AllowAuthorizationCodeFlow || input.AllowClientCredentialsFlow || input.AllowPasswordFlow || input.AllowRefreshTokenFlow || input.AllowDeviceEndpoint)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
        }

        if (input.AllowAuthorizationCodeFlow)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
        }

        if (input.AllowImplicitFlow)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
            if (string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
            }
        }

        foreach (var redirectUri in (input.RedirectUris ?? new HashSet<string>()).Select((x) => new Uri(x, UriKind.Absolute)))
        {
            applicationDescriptor.RedirectUris.Add(redirectUri);
        }

        foreach (var postLogoutRedirectUri in (input.PostLogoutRedirectUris ?? new HashSet<string>()).Select((x) => new Uri(x, UriKind.Absolute)))
        {
            applicationDescriptor.PostLogoutRedirectUris.Add(postLogoutRedirectUri);
        }

        if (input.ExtensionGrantTypes == null)
        {
            input.ExtensionGrantTypes = new HashSet<string>();
        }
        else
        {
            input.ExtensionGrantTypes.RemoveAll(x => x == OpenIddictConstants.Permissions.GrantTypes.RefreshToken || x == OpenIddictConstants.Permissions.GrantTypes.Password || x == OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode || x == OpenIddictConstants.Permissions.GrantTypes.ClientCredentials || x == OpenIddictConstants.Permissions.GrantTypes.DeviceCode || x == OpenIddictConstants.Permissions.GrantTypes.Implicit);
        }

        foreach (string extensionGrantType in input.ExtensionGrantTypes)
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.GrantType + extensionGrantType);
        }

        foreach (var scope in input.Scopes ?? new HashSet<string>())
        {
            applicationDescriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
        }

        var application = await ApplicationManager.CreateAsync(applicationDescriptor);
        input.MapExtraPropertiesTo(application.As<OpenIddictApplicationModel>());
        await ApplicationManager.UpdateAsync(application);
        return ObjectMapper.Map<OpenIddictApplicationModel, ApplicationDto>(application.As<OpenIddictApplicationModel>());
    }

    [Authorize(AbpOpenIddictProPermissions.Application.Update)]
    public virtual async Task<ApplicationDto> UpdateAsync(Guid id, UpdateApplicationInput input)
    {
        var application = (await ApplicationManager.FindByIdAsync(ConvertIdentifierToString(id)))
            .As<OpenIddictApplicationModel>() ?? throw new EntityNotFoundException(typeof(OpenIddictApplicationModel), id);

        await CheckInputDtoAsync(input, application);

        AbpApplicationDescriptor descriptor = new();

        await ApplicationManager.PopulateAsync(descriptor, application);

        descriptor.ApplicationType = input.ApplicationType;
        descriptor.ClientId = input.ClientId;
        descriptor.ConsentType = input.ConsentType;
        descriptor.DisplayName = input.DisplayName;
        descriptor.ClientType = input.ClientType;
        descriptor.ClientUri = input.ClientUri;
        descriptor.LogoUri = input.LogoUri;

        if (!string.IsNullOrEmpty(input.ClientSecret))
        {
            descriptor.ClientSecret = input.ClientSecret;
        }

        if (string.Equals(descriptor.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
        {
            descriptor.ClientSecret = null;
        }

        if (input.AllowHybridFlow)
        {
            input.AllowAuthorizationCodeFlow = true;
            input.AllowImplicitFlow = true;
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);
            if (string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
            }
            else
            {
                descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
                descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
            }
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.CodeIdToken);
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.CodeIdTokenToken);
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.CodeToken);
        }

        if (input.AllowDeviceEndpoint)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Device);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.Endpoints.Device);
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.GrantTypes.DeviceCode);
        }

        if (input.AllowAuthorizationCodeFlow)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode);
        }

        if (input.AllowClientCredentialsFlow)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials);
        }

        if (input.AllowImplicitFlow)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Implicit);
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.GrantTypes.Implicit);
        }

        if (input.AllowPasswordFlow)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.Password);
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.GrantTypes.Password);
        }

        if (input.AllowRefreshTokenFlow)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.GrantTypes.RefreshToken);
        }

        if (!input.AllowAuthorizationCodeFlow && !input.AllowImplicitFlow)
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.Endpoints.Authorization);
        }
        else
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Authorization);
        }

        if (!input.AllowAuthorizationCodeFlow && !input.AllowClientCredentialsFlow && !input.AllowPasswordFlow && !input.AllowRefreshTokenFlow && !input.AllowDeviceEndpoint)
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.Endpoints.Token);
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.Endpoints.Revocation);
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.Endpoints.Introspection);
        }
        else
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Token);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Revocation);
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Introspection);
        }

        if (input.AllowAuthorizationCodeFlow)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Code);
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.Code);
        }

        if (input.AllowImplicitFlow)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
            if (string.Equals(input.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
            {
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                descriptor.Permissions.Add(OpenIddictConstants.Permissions.ResponseTypes.Token);
            }
            else
            {
                descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
                descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.Token);
            }
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.IdToken);
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.IdTokenToken);
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.ResponseTypes.Token);
        }

        descriptor.RedirectUris.Clear();
        foreach (var redirectUri in (input.RedirectUris ?? new HashSet<string>()).Select((x) => new Uri(x, UriKind.Absolute)))
        {
            descriptor.RedirectUris.Add(redirectUri);
        }

        descriptor.PostLogoutRedirectUris.Clear();
        foreach (var postLogoutRedirectUri in (input.PostLogoutRedirectUris ?? new HashSet<string>()).Select((x) => new Uri(x, UriKind.Absolute)))
        {
            descriptor.PostLogoutRedirectUris.Add(postLogoutRedirectUri);
        }

        descriptor.Permissions.RemoveAll(x => x.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType, StringComparison.OrdinalIgnoreCase) && x != OpenIddictConstants.Permissions.GrantTypes.RefreshToken && x != OpenIddictConstants.Permissions.GrantTypes.Password && x != OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode && x != OpenIddictConstants.Permissions.GrantTypes.ClientCredentials && x != OpenIddictConstants.Permissions.GrantTypes.DeviceCode && x != OpenIddictConstants.Permissions.GrantTypes.Implicit);
        foreach (string extensionGrantType in input.ExtensionGrantTypes ?? new HashSet<string>())
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.GrantType + extensionGrantType);
        }

        if (input.AllowLogoutEndpoint)
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Endpoints.Logout);
        }
        else
        {
            descriptor.Permissions.Remove(OpenIddictConstants.Permissions.Endpoints.Logout);
        }

        descriptor.Permissions.RemoveWhere((permission) => permission.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope, StringComparison.InvariantCultureIgnoreCase));
        foreach (var scope in input.Scopes ?? new HashSet<string>())
        {
            descriptor.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + scope);
        }

        input.MapExtraPropertiesTo(application.As<OpenIddictApplicationModel>());
        await ApplicationManager.UpdateAsync(application, descriptor);
        return ObjectMapper.Map<OpenIddictApplicationModel, ApplicationDto>(application);
    }

    [Authorize(AbpOpenIddictProPermissions.Application.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var application = (await ApplicationManager.FindByIdAsync(ConvertIdentifierToString(id))) ?? throw new EntityNotFoundException(typeof(OpenIddictApplicationModel), id);
        await ApplicationManager.DeleteAsync(application);
    }

    protected virtual async Task CheckInputDtoAsync(ApplicationCreateOrUpdateDtoBase dto, OpenIddictApplicationModel application = null)
    {
        if (!dto.RedirectUris.IsNullOrEmpty())
        {
            foreach (var item in from x in dto.RedirectUris
                                 select x)
            {
                if (!Uri.TryCreate(item, UriKind.Absolute, out var result) || !result.IsWellFormedOriginalString())
                {
                    throw new UserFriendlyException(L["InvalidRedirectUri", item]);
                }
            }
        }

        if (!dto.PostLogoutRedirectUris.IsNullOrEmpty())
        {
            foreach (var item in dto.PostLogoutRedirectUris)
            {
                if (!Uri.TryCreate(item, UriKind.Absolute, out var result) || !result.IsWellFormedOriginalString())
                {
                    throw new UserFriendlyException(L["InvalidPostLogoutRedirectUri", item]);
                }
            }
        }

        if (!dto.ClientSecret.IsNullOrEmpty() && string.Equals(dto.ClientType, OpenIddictConstants.ClientTypes.Public, StringComparison.OrdinalIgnoreCase))
        {
            throw new UserFriendlyException(L["NoClientSecretCanBeSetForPublicApplications"]);
        }

        if (dto.ClientSecret.IsNullOrEmpty() && string.Equals(dto.ClientType, OpenIddictConstants.ClientTypes.Confidential, StringComparison.OrdinalIgnoreCase) && (application == null || application.ClientSecret.IsNullOrEmpty()))
        {
            throw new UserFriendlyException(L["TheClientSecretIsRequiredForConfidentialApplications"]);
        }

        bool flag;
        if (flag = !dto.ClientId.IsNullOrEmpty())
        {
            flag = await ApplicationManager.FindByClientIdAsync(dto.ClientId) != null;
        }

        if (flag && (application == null || application.ClientId != dto.ClientId))
        {
            throw new UserFriendlyException(L["TheClientIdentifierIsAlreadyTakenByAnotherApplication"]);
        }
    }
}
