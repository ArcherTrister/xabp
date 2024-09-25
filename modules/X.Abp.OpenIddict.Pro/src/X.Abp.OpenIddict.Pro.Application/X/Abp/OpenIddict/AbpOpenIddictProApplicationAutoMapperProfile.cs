// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;

using AutoMapper;

using OpenIddict.Abstractions;

using Volo.Abp.OpenIddict.Applications;
using Volo.Abp.OpenIddict.Scopes;

using X.Abp.OpenIddict.Applications.Dtos;
using X.Abp.OpenIddict.Scopes.Dtos;

namespace X.Abp.OpenIddict;

public class AbpOpenIddictProApplicationAutoMapperProfile : Profile
{
    public AbpOpenIddictProApplicationAutoMapperProfile()
    {
        CreateMap<OpenIddictApplicationModel, ApplicationDto>(MemberList.Destination)
                  .MapExtraProperties()
                  .ForMember(applicationDto => applicationDto.ClientSecret, x => x.Ignore())
                  .ForMember(applicationDto => applicationDto.AllowAuthorizationCodeFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode)))
                  .ForMember(applicationDto => applicationDto.AllowClientCredentialsFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.ClientCredentials)))
                  .ForMember(applicationDto => applicationDto.AllowImplicitFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.Implicit)))
                  .ForMember(applicationDto => applicationDto.AllowHybridFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode) && applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.Implicit)))
                  .ForMember(applicationDto => applicationDto.AllowPasswordFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.Password)))
                  .ForMember(applicationDto => applicationDto.AllowRefreshTokenFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.GrantTypes.RefreshToken)))
                  .ForMember(applicationDto => applicationDto.AllowLogoutEndpoint, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.Endpoints.Logout)))
                  .ForMember(applicationDto => applicationDto.AllowDeviceEndpoint, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains(OpenIddictConstants.Permissions.Endpoints.Device)))
                  .ForMember(applicationDto => applicationDto.RedirectUris, x => x.MapFrom(applicationModel => HashSet(applicationModel.RedirectUris)))
                  .ForMember(applicationDto => applicationDto.PostLogoutRedirectUris, x => x.MapFrom(applicationModel => HashSet(applicationModel.PostLogoutRedirectUris)))
                  .ForMember(applicationDto => applicationDto.ExtensionGrantTypes, x => x.MapFrom(applicationModel => HashSet(applicationModel.Permissions)
                        .Where(str => str != OpenIddictConstants.Permissions.GrantTypes.RefreshToken && str != OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode && str != OpenIddictConstants.Permissions.GrantTypes.Implicit && str != OpenIddictConstants.Permissions.GrantTypes.Password && str != OpenIddictConstants.Permissions.GrantTypes.ClientCredentials && str != OpenIddictConstants.Permissions.GrantTypes.DeviceCode)
                        .Where(str => str.StartsWith(OpenIddictConstants.Permissions.Prefixes.GrantType, StringComparison.OrdinalIgnoreCase))
                        .Select(str => str.Substring(OpenIddictConstants.Permissions.Prefixes.GrantType.Length))))
                  .ForMember(applicationDto => applicationDto.Scopes, x => x.MapFrom(applicationModel => HashSet(applicationModel.Permissions).Where(str => str.StartsWith(OpenIddictConstants.Permissions.Prefixes.Scope, StringComparison.InvariantCultureIgnoreCase)).Select(str => str.Substring(OpenIddictConstants.Permissions.Prefixes.Scope.Length))));

        CreateMap<OpenIddictScopeModel, ScopeDto>(MemberList.Destination)
                  .MapExtraProperties()
                  .ForMember(scopeDto => scopeDto.BuildIn, x => x.Ignore())
                  .ForMember(scopeDto => scopeDto.Resources, x => x.MapFrom(iddictScopeModel => HashSet(iddictScopeModel.Resources)));
    }

    private static HashSet<string> HashSet(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return ImmutableArray.Create<string>().ToHashSet();
        }

        using var jsonDocument = JsonDocument.Parse(value);
        var builder = ImmutableArray.CreateBuilder<string>(jsonDocument.RootElement.GetArrayLength());
        foreach (var enumerate in jsonDocument.RootElement.EnumerateArray())
        {
            var str = enumerate.GetString();
            if (!string.IsNullOrEmpty(str))
            {
                builder.Add(str);
            }
        }

        return builder.ToImmutable().ToHashSet();
    }
}
