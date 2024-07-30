// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.IdentityServer.Client.Dtos;

public class UpdateClientDto : ExtensibleObject
{
    [DynamicStringLength(typeof(ClientConsts), "ClientNameMaxLength", null)]
    public string ClientName { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "DescriptionMaxLength", null)]
    public string Description { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "ClientUriMaxLength", null)]
    public string ClientUri { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "LogoUriMaxLength", null)]
    public string LogoUri { get; set; }

    public bool Enabled { get; set; } = true;

    public bool RequireConsent { get; set; }

    public bool AllowOfflineAccess { get; set; }

    public bool AllowRememberConsent { get; set; }

    public bool AllowAccessTokensViaBrowser { get; set; }

    public bool RequirePkce { get; set; }

    public bool RequireClientSecret { get; set; }

    public bool RequireRequestObject { get; set; }

    public int AccessTokenLifetime { get; set; }

    public int? ConsentLifetime { get; set; }

    public int AccessTokenType { get; set; }

    public bool EnableLocalLogin { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "FrontChannelLogoutUriMaxLength", null)]
    public string FrontChannelLogoutUri { get; set; }

    public bool FrontChannelLogoutSessionRequired { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "BackChannelLogoutUriMaxLength", null)]
    public string BackChannelLogoutUri { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "AllowedIdentityTokenSigningAlgorithms", null)]
    public string AllowedIdentityTokenSigningAlgorithms { get; set; }

    public bool BackChannelLogoutSessionRequired { get; set; }

    public bool IncludeJwtId { get; set; }

    public bool AlwaysSendClientClaims { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "PairWiseSubjectSaltMaxLength", null)]
    public string PairWiseSubjectSalt { get; set; }

    public int? UserSsoLifetime { get; set; }

    [DynamicStringLength(typeof(ClientConsts), "UserCodeTypeMaxLength", null)]
    public string UserCodeType { get; set; }

    public int DeviceCodeLifetime { get; set; }

    public ClientSecretDto[] Secrets { get; set; }

    public ClientClaimDto[] Claims { get; set; }

    public ClientPropertyDto[] Properties { get; set; }

    public string[] AllowedGrantTypes { get; set; }

    public string[] IdentityProviderRestrictions { get; set; }

    public string[] AllowedScopes { get; set; }

    public string[] AllowedCorsOrigins { get; set; }

    public string[] RedirectUris { get; set; }

    public string[] PostLogoutRedirectUris { get; set; }
}
