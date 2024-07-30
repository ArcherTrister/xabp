// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

using X.Abp.IdentityServer.Client.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.Clients
{
    public class ClientUpdateModalView : ExtensibleObject
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "ClientNameMaxLength", null)]
        public string ClientName { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "ClientUriMaxLength", null)]
        public string ClientUri { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "LogoUriMaxLength", null)]
        public string LogoUri { get; set; }

        public bool RequireConsent { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "PairWiseSubjectSaltMaxLength", null)]
        public string PairWiseSubjectSalt { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "AllowedIdentityTokenSigningAlgorithms", null)]
        public string AllowedIdentityTokenSigningAlgorithms { get; set; }

        public bool EnableLocalLogin { get; set; }

        public bool AllowOfflineAccess { get; set; }

        public bool AllowRememberConsent { get; set; }

        public bool AlwaysSendClientClaims { get; set; }

        public bool AllowAccessTokensViaBrowser { get; set; }

        public bool Enabled { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "FrontChannelLogoutUriMaxLength", null)]
        public string FrontChannelLogoutUri { get; set; }

        public bool FrontChannelLogoutSessionRequired { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "BackChannelLogoutUriMaxLength", null)]
        public string BackChannelLogoutUri { get; set; }

        public bool BackChannelLogoutSessionRequired { get; set; }

        public bool IncludeJwtId { get; set; }

        public bool RequirePkce { get; set; }

        public bool RequireRequestObject { get; set; }

        public bool RequireClientSecret { get; set; }

        public int AccessTokenLifetime { get; set; }

        public int AccessTokenType { get; set; }

        public int ConsentLifetime { get; set; }

        [TextArea]
        [DynamicStringLength(typeof(ClientConsts), "DescriptionMaxLength", null)]
        public string Description { get; set; }

        public int? UserSsoLifetime { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "UserCodeTypeMaxLength", null)]
        public string UserCodeType { get; set; }

        public int DeviceCodeLifetime { get; set; }

        public ClientClaimDto[] Claims { get; set; }

        public ClientSecretDto[] ClientSecrets { get; set; }

        public ClientPropertyDto[] Properties { get; set; }

        public string[] AllowedGrantTypes { get; set; }

        public string[] IdentityProviderRestrictions { get; set; }

        public string[] Scopes { get; set; }

        public string[] AllowedCorsOrigins { get; set; }

        public string[] RedirectUris { get; set; }

        public string[] PostLogoutRedirectUris { get; set; }

        public string[] ApiResourceScopes { get; set; }

        public void TrimUrls()
        {
            LogoUri = LogoUri?.Trim();
            ClientUri = ClientUri?.Trim();
            if (RedirectUris != null)
            {
                for (int i = 0; i < RedirectUris.Length; i++)
                {
                    if (RedirectUris[i] != null)
                    {
                        RedirectUris[i] = RedirectUris[i]?.Trim();
                    }
                }
            }
        }

        public ClientUpdateModalView()
        {
            Properties = Array.Empty<ClientPropertyDto>();
            AllowedGrantTypes = Array.Empty<string>();
            IdentityProviderRestrictions = Array.Empty<string>();
            Scopes = Array.Empty<string>();
            AllowedCorsOrigins = Array.Empty<string>();
            RedirectUris = Array.Empty<string>();
            PostLogoutRedirectUris = Array.Empty<string>();
            ApiResourceScopes = Array.Empty<string>();
        }
    }
}
