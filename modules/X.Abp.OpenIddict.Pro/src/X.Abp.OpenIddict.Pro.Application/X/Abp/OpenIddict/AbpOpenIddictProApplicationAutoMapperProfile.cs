// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;

using AutoMapper;

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
                  .ForMember(applicationDto => applicationDto.AllowAuthorizationCodeFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains("gt:authorization_code")))
                  .ForMember(applicationDto => applicationDto.AllowClientCredentialsFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains("gt:client_credentials")))
                  .ForMember(applicationDto => applicationDto.AllowImplicitFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains("gt:implicit")))
                  .ForMember(applicationDto => applicationDto.AllowHybridFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains("gt:authorization_code") && applicationModel.Permissions.Contains("gt:implicit")))
                  .ForMember(applicationDto => applicationDto.AllowPasswordFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains("gt:password")))
                  .ForMember(applicationDto => applicationDto.AllowRefreshTokenFlow, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains("gt:refresh_token")))
                  .ForMember(applicationDto => applicationDto.AllowLogoutEndpoint, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains("ept:logout")))
                  .ForMember(applicationDto => applicationDto.AllowDeviceEndpoint, x => x.MapFrom(applicationModel => applicationModel.Permissions.Contains("ept:device")))
                  .ForMember(applicationDto => applicationDto.RedirectUris, x => x.MapFrom(applicationModel => HashSet(applicationModel.RedirectUris)))
                  .ForMember(applicationDto => applicationDto.PostLogoutRedirectUris, x => x.MapFrom(applicationModel => HashSet(applicationModel.PostLogoutRedirectUris)))
                  .ForMember(applicationDto => applicationDto.ExtensionGrantTypes, x => x.MapFrom(applicationModel => HashSet(applicationModel.Permissions)
                        .Where(str => str != "gt:refresh_token" && str != "gt:authorization_code" && str != "gt:implicit" && str != "gt:password" && str != "gt:client_credentials" && str != "gt:urn:ietf:params:oauth:grant-type:device_code")
                        .Where(str => str.StartsWith("gt:", StringComparison.OrdinalIgnoreCase))
                        .Select(str => str.Substring("gt:".Length))))
                  .ForMember(applicationDto => applicationDto.Scopes, x => x.MapFrom(applicationModel => HashSet(applicationModel.Permissions).Where(str => str.StartsWith("scp:", StringComparison.InvariantCultureIgnoreCase)).Select(str => str.Substring("scp:".Length))));

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
