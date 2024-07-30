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
            applicationDescriptor.Permissions.Add("rst:code id_token");
            if (string.Equals(input.ClientType, "public", StringComparison.OrdinalIgnoreCase))
            {
                applicationDescriptor.Permissions.Add("rst:code id_token token");
                applicationDescriptor.Permissions.Add("rst:code token");
            }
        }

        if (input.AllowLogoutEndpoint)
        {
            applicationDescriptor.Permissions.Add("ept:logout");
        }

        if (input.AllowDeviceEndpoint)
        {
            applicationDescriptor.Permissions.Add("ept:device");
            applicationDescriptor.Permissions.Add("gt:urn:ietf:params:oauth:grant-type:device_code");
        }

        if (input.AllowAuthorizationCodeFlow)
        {
            applicationDescriptor.Permissions.Add("gt:authorization_code");
        }

        if (input.AllowClientCredentialsFlow)
        {
            applicationDescriptor.Permissions.Add("gt:client_credentials");
        }

        if (input.AllowImplicitFlow)
        {
            applicationDescriptor.Permissions.Add("gt:implicit");
        }

        if (input.AllowPasswordFlow)
        {
            applicationDescriptor.Permissions.Add("gt:password");
        }

        if (input.AllowRefreshTokenFlow)
        {
            applicationDescriptor.Permissions.Add("gt:refresh_token");
        }

        if (input.AllowAuthorizationCodeFlow || input.AllowImplicitFlow)
        {
            applicationDescriptor.Permissions.Add("ept:authorization");
        }

        if (input.AllowAuthorizationCodeFlow || input.AllowClientCredentialsFlow || input.AllowPasswordFlow || input.AllowRefreshTokenFlow || input.AllowDeviceEndpoint)
        {
            applicationDescriptor.Permissions.Add("ept:token");
            applicationDescriptor.Permissions.Add("ept:revocation");
            applicationDescriptor.Permissions.Add("ept:introspection");
        }

        if (input.AllowAuthorizationCodeFlow)
        {
            applicationDescriptor.Permissions.Add("rst:code");
        }

        if (input.AllowImplicitFlow)
        {
            applicationDescriptor.Permissions.Add("rst:id_token");
            if (string.Equals(input.ClientType, "public", StringComparison.OrdinalIgnoreCase))
            {
                applicationDescriptor.Permissions.Add("rst:id_token token");
                applicationDescriptor.Permissions.Add("rst:token");
            }
        }

        foreach (var redirectUri in (input.RedirectUris ?? new HashSet<string>()).Select((string x) => new Uri(x, UriKind.Absolute)))
        {
            applicationDescriptor.RedirectUris.Add(redirectUri);
        }

        foreach (var postLogoutRedirectUri in (input.PostLogoutRedirectUris ?? new HashSet<string>()).Select((string x) => new Uri(x, UriKind.Absolute)))
        {
            applicationDescriptor.PostLogoutRedirectUris.Add(postLogoutRedirectUri);
        }

        if (input.ExtensionGrantTypes == null)
        {
            input.ExtensionGrantTypes = new HashSet<string>();
        }
        else
        {
            input.ExtensionGrantTypes.RemoveAll(x => x == "gt:refresh_token" || x == "gt:password" || x == "gt:authorization_code" || x == "gt:client_credentials" || x == "gt:urn:ietf:params:oauth:grant-type:device_code" || x == "gt:implicit");
        }

        foreach (string extensionGrantType in input.ExtensionGrantTypes)
        {
            applicationDescriptor.Permissions.Add("gt:" + extensionGrantType);
        }

        foreach (var scope in input.Scopes ?? new HashSet<string>())
        {
            applicationDescriptor.Permissions.Add("scp:" + scope);
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

        if (string.Equals(descriptor.ClientType, "public", StringComparison.OrdinalIgnoreCase))
        {
            descriptor.ClientSecret = null;
        }

        if (input.AllowHybridFlow)
        {
            input.AllowAuthorizationCodeFlow = true;
            input.AllowImplicitFlow = true;
            descriptor.Permissions.Add("rst:code id_token");
            if (string.Equals(input.ClientType, "public", StringComparison.OrdinalIgnoreCase))
            {
                descriptor.Permissions.Add("rst:code id_token token");
                descriptor.Permissions.Add("rst:code token");
            }
            else
            {
                descriptor.Permissions.Remove("rst:code id_token token");
                descriptor.Permissions.Remove("rst:code token");
            }
        }
        else
        {
            descriptor.Permissions.Remove("rst:code id_token");
            descriptor.Permissions.Remove("rst:code id_token token");
            descriptor.Permissions.Remove("rst:code token");
        }

        if (input.AllowDeviceEndpoint)
        {
            descriptor.Permissions.Add("ept:device");
            descriptor.Permissions.Add("gt:urn:ietf:params:oauth:grant-type:device_code");
        }
        else
        {
            descriptor.Permissions.Remove("ept:device");
            descriptor.Permissions.Remove("gt:urn:ietf:params:oauth:grant-type:device_code");
        }

        if (input.AllowAuthorizationCodeFlow)
        {
            descriptor.Permissions.Add("gt:authorization_code");
        }
        else
        {
            descriptor.Permissions.Remove("gt:authorization_code");
        }

        if (input.AllowClientCredentialsFlow)
        {
            descriptor.Permissions.Add("gt:client_credentials");
        }
        else
        {
            descriptor.Permissions.Remove("gt:client_credentials");
        }

        if (input.AllowImplicitFlow)
        {
            descriptor.Permissions.Add("gt:implicit");
        }
        else
        {
            descriptor.Permissions.Remove("gt:implicit");
        }

        if (input.AllowPasswordFlow)
        {
            descriptor.Permissions.Add("gt:password");
        }
        else
        {
            descriptor.Permissions.Remove("gt:password");
        }

        if (input.AllowRefreshTokenFlow)
        {
            descriptor.Permissions.Add("gt:refresh_token");
        }
        else
        {
            descriptor.Permissions.Remove("gt:refresh_token");
        }

        if (!input.AllowAuthorizationCodeFlow && !input.AllowImplicitFlow)
        {
            descriptor.Permissions.Remove("ept:authorization");
        }
        else
        {
            descriptor.Permissions.Add("ept:authorization");
        }

        if (!input.AllowAuthorizationCodeFlow && !input.AllowClientCredentialsFlow && !input.AllowPasswordFlow && !input.AllowRefreshTokenFlow && !input.AllowDeviceEndpoint)
        {
            descriptor.Permissions.Remove("ept:token");
            descriptor.Permissions.Remove("ept:revocation");
            descriptor.Permissions.Remove("ept:introspection");
        }
        else
        {
            descriptor.Permissions.Add("ept:token");
            descriptor.Permissions.Add("ept:revocation");
            descriptor.Permissions.Add("ept:introspection");
        }

        if (input.AllowAuthorizationCodeFlow)
        {
            descriptor.Permissions.Add("rst:code");
        }
        else
        {
            descriptor.Permissions.Remove("rst:code");
        }

        if (input.AllowImplicitFlow)
        {
            descriptor.Permissions.Add("rst:id_token");
            if (string.Equals(input.ClientType, "public", StringComparison.OrdinalIgnoreCase))
            {
                descriptor.Permissions.Add("rst:id_token token");
                descriptor.Permissions.Add("rst:token");
            }
            else
            {
                descriptor.Permissions.Remove("rst:id_token token");
                descriptor.Permissions.Remove("rst:token");
            }
        }
        else
        {
            descriptor.Permissions.Remove("rst:id_token");
            descriptor.Permissions.Remove("rst:id_token token");
            descriptor.Permissions.Remove("rst:token");
        }

        descriptor.RedirectUris.Clear();
        foreach (var redirectUri in (input.RedirectUris ?? new HashSet<string>()).Select((string x) => new Uri(x, UriKind.Absolute)))
        {
            descriptor.RedirectUris.Add(redirectUri);
        }

        descriptor.PostLogoutRedirectUris.Clear();
        foreach (var postLogoutRedirectUri in (input.PostLogoutRedirectUris ?? new HashSet<string>()).Select((string x) => new Uri(x, UriKind.Absolute)))
        {
            descriptor.PostLogoutRedirectUris.Add(postLogoutRedirectUri);
        }

        descriptor.Permissions.RemoveAll(x => x.StartsWith("gt:", StringComparison.OrdinalIgnoreCase) && x != "gt:refresh_token" && x != "gt:password" && x != "gt:authorization_code" && x != "gt:client_credentials" && x != "gt:urn:ietf:params:oauth:grant-type:device_code" && x != "gt:implicit");
        foreach (string extensionGrantType in input.ExtensionGrantTypes ?? new HashSet<string>())
        {
            descriptor.Permissions.Add("gt:" + extensionGrantType);
        }

        if (input.AllowLogoutEndpoint)
        {
            descriptor.Permissions.Add("ept:logout");
        }
        else
        {
            descriptor.Permissions.Remove("ept:logout");
        }

        descriptor.Permissions.RemoveWhere((string permission) => permission.StartsWith("scp:", StringComparison.InvariantCultureIgnoreCase));
        foreach (var scope in input.Scopes ?? new HashSet<string>())
        {
            descriptor.Permissions.Add("scp:" + scope);
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

        if (!dto.ClientSecret.IsNullOrEmpty() && string.Equals(dto.ClientType, "public", StringComparison.OrdinalIgnoreCase))
        {
            throw new UserFriendlyException(L["NoClientSecretCanBeSetForPublicApplications"]);
        }

        if (dto.ClientSecret.IsNullOrEmpty() && string.Equals(dto.ClientType, "confidential", StringComparison.OrdinalIgnoreCase) && (application == null || application.ClientSecret.IsNullOrEmpty()))
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
