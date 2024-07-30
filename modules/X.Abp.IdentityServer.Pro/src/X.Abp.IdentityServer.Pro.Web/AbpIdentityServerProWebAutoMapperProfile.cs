// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;

using X.Abp.IdentityServer.ApiResource.Dtos;
using X.Abp.IdentityServer.ApiScope.Dtos;
using X.Abp.IdentityServer.Client.Dtos;
using X.Abp.IdentityServer.IdentityResource.Dtos;
using X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiResources;
using X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiScopes;
using X.Abp.IdentityServer.Web.Pages.IdentityServer.Clients;
using X.Abp.IdentityServer.Web.Pages.IdentityServer.IdentityResources;

namespace X.Abp.IdentityServer.Web;

public class AbpIdentityServerProWebAutoMapperProfile : Profile
{
    public AbpIdentityServerProWebAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<IdentityResourceCreateModalView, CreateIdentityResourceDto>()
            .Ignore(d => d.UserClaims)
            .Ignore(d => d.Properties)
            .MapExtraProperties();

        CreateMap<IdentityResourceWithDetailsDto, IdentityResourceUpdateModalView>()
            .Ignore(d => d.UserClaims)
            .Ignore(d => d.Properties)
            .MapExtraProperties();

        CreateMap<IdentityResourceUpdateModalView, UpdateIdentityResourceDto>()
            .Ignore(d => d.UserClaims)
            .Ignore(d => d.Properties)
            .MapExtraProperties();

        CreateMap<ApiResourceCreateModalView, CreateApiResourceDto>()
            .Ignore(d => d.UserClaims)
            .MapExtraProperties();

        CreateMap<ApiResourceUpdateModalView, UpdateApiResourceDto>()
            .Ignore(d => d.UserClaims)
            .Ignore(d => d.Scopes)
            .Ignore(d => d.Properties)
            .MapExtraProperties();

        CreateMap<ApiResourceWithDetailsDto, ApiResourceUpdateModalView>()
            .Ignore(d => d.UserClaims)
            .Ignore(d => d.Scopes)
            .Ignore(d => d.Properties)
            .Ignore(d => d.Secrets)
            .MapExtraProperties();

        CreateMap<ApiScopeCreateModalView, CreateApiScopeDto>()
            .Ignore(d => d.UserClaims)
            .Ignore(d => d.Properties)
            .MapExtraProperties();

        CreateMap<ApiScopeWithDetailsDto, ApiScopeUpdateModalView>()
            .Ignore(d => d.UserClaims)
            .MapExtraProperties();

        CreateMap<ApiScopeUpdateModalView, UpdateApiScopeDto>()
            .Ignore(d => d.UserClaims)
            .Ignore(d => d.Properties)
            .MapExtraProperties();

        CreateMap<ClientWithDetailsDto, ClientUpdateModalView>()
            .Ignore(d => d.Claims)
            .Ignore(d => d.ClientSecrets)
            .Ignore(d => d.AllowedGrantTypes)
            .Ignore(d => d.IdentityProviderRestrictions)
            .Ignore(d => d.Properties)
            .Ignore(d => d.Scopes)
            .Ignore(d => d.ApiResourceScopes)
            .Ignore(d => d.PostLogoutRedirectUris)
            .Ignore(d => d.AllowedCorsOrigins)
            .Ignore(d => d.RedirectUris)
            .MapExtraProperties();

        CreateMap<ClientUpdateModalView, UpdateClientDto>()
            .Ignore(d => d.Secrets)
            .MapExtraProperties();

        CreateMap<ClientCreateModalView, CreateClientDto>()
            .Ignore(d => d.Secrets)
            .Ignore(d => d.Scopes)
            .MapExtraProperties();
    }
}
