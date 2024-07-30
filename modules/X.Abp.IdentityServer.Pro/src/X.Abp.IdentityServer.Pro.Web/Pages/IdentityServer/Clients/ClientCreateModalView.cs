// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;
using Volo.Abp.IdentityServer.Clients;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

using X.Abp.IdentityServer.Client.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.Clients
{
    public class ClientCreateModalView : ExtensibleObject
    {
        [Required]
        public string ClientId { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "ClientNameMaxLength", null)]
        [Required]
        public string ClientName { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "DescriptionMaxLength", null)]
        [TextArea]
        public string Description { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "ClientUriMaxLength", null)]
        public string ClientUri { get; set; }

        [DynamicStringLength(typeof(ClientConsts), "LogoUriMaxLength", null)]
        public string LogoUri { get; set; }

        public bool RequireConsent { get; set; }

        public string CallbackUrl { get; set; }

        public ClientSecretDto[] Secrets { get; set; }

        public string[] Scopes { get; set; }

        public string[] ApiResourceScopes { get; set; }

        public string LogoutUrl { get; set; }

        public void TrimUrls()
        {
            ClientUri = ClientUri?.Trim();
            LogoUri = LogoUri?.Trim();
            CallbackUrl = CallbackUrl?.Trim();
            LogoutUrl = LogoutUrl?.Trim();
        }

        public ClientCreateModalView()
        {
            Scopes = System.Array.Empty<string>();
            ApiResourceScopes = System.Array.Empty<string>();
        }
    }
}
