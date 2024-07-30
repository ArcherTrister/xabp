// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.Identity;
using Volo.Abp.IdentityServer.ApiResources;
using Volo.Abp.IdentityServer.ApiScopes;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.IdentityServer.IdentityResources;

using X.Abp.IdentityServer.ApiResource.Dtos;
using X.Abp.IdentityServer.ApiScope.Dtos;
using X.Abp.IdentityServer.ClaimType.Dtos;
using X.Abp.IdentityServer.Client.Dtos;
using X.Abp.IdentityServer.IdentityResource.Dtos;

namespace X.Abp.IdentityServer;

public class AbpIdentityServerProApplicationAutoMapperProfile : Profile
{
    public AbpIdentityServerProApplicationAutoMapperProfile()
    {
        CreateMap<Volo.Abp.IdentityServer.ApiScopes.ApiScope, ApiScopeWithDetailsDto>().MapExtraProperties();
        CreateMap<ApiScopeClaim, ApiScopeClaimDto>();
        CreateMap<ApiScopeProperty, ApiScopePropertyDto>();
        CreateMap<Volo.Abp.IdentityServer.IdentityResources.IdentityResource, IdentityResourceWithDetailsDto>().MapExtraProperties();
        CreateMap<IdentityResourceClaim, IdentityResourceClaimDto>();
        CreateMap<IdentityResourceProperty, IdentityResourcePropertyDto>();
        CreateMap<IdentityClaimType, IdentityClaimTypeDto>().MapExtraProperties();
        CreateMap<Volo.Abp.IdentityServer.ApiResources.ApiResource, ApiResourceWithDetailsDto>().MapExtraProperties();
        CreateMap<ApiResourceScope, ApiResourceScopeDto>();
        CreateMap<ApiResourceSecret, ApiResourceSecretDto>();
        CreateMap<ApiResourceClaim, ApiResourceClaimDto>();
        CreateMap<ApiResourceProperty, ApiResourcePropertyDto>();
        CreateMap<Volo.Abp.IdentityServer.Clients.Client, ClientWithDetailsDto>().MapExtraProperties();
        CreateMap<ClientSecret, ClientSecretDto>();
        CreateMap<ClientScope, ClientScopeDto>();
        CreateMap<ClientClaim, ClientClaimDto>();
        CreateMap<ClientProperty, ClientPropertyDto>();
        CreateMap<ClientRedirectUri, ClientRedirectUriDto>();
        CreateMap<ClientPostLogoutRedirectUri, ClientPostLogoutRedirectUriDto>();
        CreateMap<ClientIdPRestriction, ClientIdentityProviderRestrictionDto>();
        CreateMap<ClientGrantType, ClientGrantTypeDto>();
        CreateMap<ClientCorsOrigin, ClientCorsOriginDto>();
    }
}
