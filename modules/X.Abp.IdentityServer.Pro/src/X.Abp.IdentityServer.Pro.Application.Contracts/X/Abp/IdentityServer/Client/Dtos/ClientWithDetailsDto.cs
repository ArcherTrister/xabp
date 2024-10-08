﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace X.Abp.IdentityServer.Client.Dtos;

public class ClientWithDetailsDto : ExtensibleEntityDto<Guid>
{
    public string ClientId { get; set; }

    public string ClientName { get; set; }

    public string Description { get; set; }

    public string ClientUri { get; set; }

    public string LogoUri { get; set; }

    public bool Enabled { get; set; }

    public string ProtocolType { get; set; }

    public bool RequireClientSecret { get; set; }

    public bool RequireConsent { get; set; }

    public bool AllowRememberConsent { get; set; }

    public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

    public bool RequirePkce { get; set; }

    public bool AllowPlainTextPkce { get; set; }

    public bool RequireRequestObject { get; set; }

    public bool AllowAccessTokensViaBrowser { get; set; }

    public string FrontChannelLogoutUri { get; set; }

    public bool FrontChannelLogoutSessionRequired { get; set; }

    public string BackChannelLogoutUri { get; set; }

    public bool BackChannelLogoutSessionRequired { get; set; }

    public bool AllowOfflineAccess { get; set; }

    public int IdentityTokenLifetime { get; set; }

    public string AllowedIdentityTokenSigningAlgorithms { get; set; }

    public int AccessTokenLifetime { get; set; }

    public int AuthorizationCodeLifetime { get; set; }

    public int? ConsentLifetime { get; set; }

    public int AbsoluteRefreshTokenLifetime { get; set; }

    public int SlidingRefreshTokenLifetime { get; set; }

    public int RefreshTokenUsage { get; set; }

    public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

    public int RefreshTokenExpiration { get; set; }

    public int AccessTokenType { get; set; }

    public bool EnableLocalLogin { get; set; }

    public bool IncludeJwtId { get; set; }

    public bool AlwaysSendClientClaims { get; set; }

    public string ClientClaimsPrefix { get; set; }

    public string PairWiseSubjectSalt { get; set; }

    public int? UserSsoLifetime { get; set; }

    public string UserCodeType { get; set; }

    public int DeviceCodeLifetime { get; set; }

    public List<ClientSecretDto> ClientSecrets { get; set; }

    public List<ClientScopeDto> AllowedScopes { get; set; }

    public List<ClientClaimDto> Claims { get; set; }

    public List<ClientGrantTypeDto> AllowedGrantTypes { get; set; }

    public List<ClientIdentityProviderRestrictionDto> IdentityProviderRestrictions { get; set; }

    public List<ClientPropertyDto> Properties { get; set; }

    public List<ClientCorsOriginDto> AllowedCorsOrigins { get; set; }

    public List<ClientRedirectUriDto> RedirectUris { get; set; }

    public List<ClientPostLogoutRedirectUriDto> PostLogoutRedirectUris { get; set; }
}
