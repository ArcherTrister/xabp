// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using X.Abp.IdentityServer.ApiResource;
using X.Abp.IdentityServer.ApiResource.Dtos;
using X.Abp.IdentityServer.ClaimType;
using X.Abp.IdentityServer.Client;
using X.Abp.IdentityServer.Client.Dtos;
using X.Abp.IdentityServer.IdentityResource;
using X.Abp.IdentityServer.IdentityResource.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.Clients
{
    public partial class EditModel : IdentityServerPageModel
    {
        protected IClientAppService ClientAppService { get; }

        protected IApiResourceAppService ApiResourceAppService { get; }

        protected IIdentityResourceAppService IdentityResourceAppService { get; }

        protected IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

        [BindProperty]
        public ClientUpdateModalView Client { get; set; }

        public string[] AllClaims { get; set; }

        public ClientClaimModalView SampleClaim { get; set; } = new ClientClaimModalView();

        public ClientPropertyModalView SampleProperty { get; set; } = new ClientPropertyModalView();

        public List<ApiResourceWithDetailsDto> AllApiResources { get; set; } = new List<ApiResourceWithDetailsDto>();

        public List<string> AppUrisMerged { get; set; } = new List<string>();

        public List<IdentityResourceWithDetailsDto> AllIdentityResources { get; set; } = new List<IdentityResourceWithDetailsDto>();

        public ClientSecretModalView SampleSecret { get; set; } = new ClientSecretModalView();

        public List<SelectListItem> SecretTypes { get; set; } = new List<SelectListItem>()
        {
            new SelectListItem()
            {
                Text = "Shared Secret",
                Value = "SharedSecret",
                Selected = true
            },
            new SelectListItem()
            {
                Text = "X509 Thumbprint",
                Value = "X509Thumbprint"
            }
        };

        public string SampleIdentityProviderRestriction { get; set; }

        public string SampleApplicationUrl { get; set; }

        public string SampleCallbackUri { get; set; }

        public string SampleSignoutUri { get; set; }

        public string SampleCorsUri { get; set; }

        public EditModel(
          IClientAppService clientAppService,
          IApiResourceAppService apiResourceAppService,
          IIdentityResourceAppService identityResourceAppService,
          IIdentityServerClaimTypeAppService claimTypeAppService)
        {
            ClientAppService = clientAppService;
            ApiResourceAppService = apiResourceAppService;
            IdentityResourceAppService = identityResourceAppService;
            ClaimTypeAppService = claimTypeAppService;
        }

        public virtual async Task OnGetAsync(Guid id)
        {
            ClientWithDetailsDto client = await ClientAppService.GetAsync(id);
            Client = ObjectMapper.Map<ClientWithDetailsDto, ClientUpdateModalView>(client);
            Client.ClientSecrets = client.ClientSecrets.ToArray();
            Client.Properties = client.Properties.ToArray();
            Client.Claims = client.Claims.ToArray();
            Client.AllowedGrantTypes = client.AllowedGrantTypes.Select(g => g.GrantType).ToArray();
            Client.IdentityProviderRestrictions = client.IdentityProviderRestrictions.Select(g => g.Provider).ToArray();
            Client.Scopes = client.AllowedScopes.Select(s => s.Scope).ToArray();
            Client.AllowedCorsOrigins = client.AllowedCorsOrigins.Select(s => s.Origin).ToArray();
            Client.RedirectUris = client.RedirectUris.Select(s => s.RedirectUri).ToArray();
            Client.PostLogoutRedirectUris = client.PostLogoutRedirectUris.Select(s => s.PostLogoutRedirectUri).ToArray();
            Client.TrimUrls();
            foreach (string allowedCorsOrigin in Client.AllowedCorsOrigins)
            {
                if (!AppUrisMerged.Contains(allowedCorsOrigin))
                {
                    AppUrisMerged.Add(allowedCorsOrigin);
                }
            }

            foreach (string redirectUri in Client.RedirectUris)
            {
                if (!AppUrisMerged.Contains(redirectUri))
                {
                    AppUrisMerged.Add(redirectUri);
                }
            }

            foreach (string logoutRedirectUri in Client.PostLogoutRedirectUris)
            {
                if (!AppUrisMerged.Contains(logoutRedirectUri))
                {
                    AppUrisMerged.Add(logoutRedirectUri);
                }
            }

            AllIdentityResources = await IdentityResourceAppService.GetAllListAsync();

            AllApiResources = await ApiResourceAppService.GetAllListAsync();

            AllClaims = (await ClaimTypeAppService.GetListAsync()).Select(ct => ct.Name).ToArray();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            Client.TrimUrls();
            UpdateClientDto input = ObjectMapper.Map<ClientUpdateModalView, UpdateClientDto>(Client);

            List<string> scopes = new List<string>();
            if (Client.Scopes != null)
            {
                scopes = Client.Scopes.Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
            }

            if (Client.ApiResourceScopes != null)
            {
                var apiResourceScopes = Client.ApiResourceScopes.Where(c => !string.IsNullOrWhiteSpace(c));
                if (apiResourceScopes != null)
                {
                    scopes.AddRange(apiResourceScopes);
                }
            }

            input.AllowedScopes = scopes.ToArray();

            if (Client.Claims != null)
            {
                var claims = Client.Claims.Where(c => c != null && !string.IsNullOrWhiteSpace(c.Value)).ToArray();
                if (claims != null)
                {
                    input.Claims = claims;
                }
            }

            if (Client.ClientSecrets != null)
            {
                var clientSecrets = Client.ClientSecrets.Where(c => c != null && !string.IsNullOrWhiteSpace(c.Value)).ToArray();
                if (clientSecrets != null)
                {
                    input.Secrets = clientSecrets;
                }
            }

            if (Client.Properties != null)
            {
                var properties = Client.Properties.Where(c => c != null && !string.IsNullOrWhiteSpace(c.Value)).ToArray();
                if (properties != null)
                {
                    input.Properties = properties;
                }
            }

            if (Client.AllowedGrantTypes != null)
            {
                var allowedGrantTypes = Client.AllowedGrantTypes.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                if (allowedGrantTypes != null)
                {
                    input.AllowedGrantTypes = allowedGrantTypes;
                }
            }

            if (Client.IdentityProviderRestrictions != null)
            {
                var providerRestrictions = Client.IdentityProviderRestrictions.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
                if (providerRestrictions != null)
                {
                    input.IdentityProviderRestrictions = providerRestrictions;
                }
            }

            if (Client.AllowedCorsOrigins != null)
            {
                var allowedCorsOrigins = Client.AllowedCorsOrigins.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
                if (allowedCorsOrigins != null)
                {
                    input.AllowedCorsOrigins = allowedCorsOrigins;
                }
            }

            if (Client.RedirectUris != null)
            {
                var redirectUris = Client.RedirectUris.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
                if (redirectUris != null)
                {
                    input.RedirectUris = redirectUris;
                }
            }

            if (Client.PostLogoutRedirectUris != null)
            {
                var logoutRedirectUris = Client.PostLogoutRedirectUris.Where(c => !string.IsNullOrWhiteSpace(c)).ToArray();
                if (logoutRedirectUris != null)
                {
                    input.PostLogoutRedirectUris = logoutRedirectUris;
                }
            }

            await ClientAppService.UpdateAsync(Client.Id, input);
            return NoContent();
        }
    }
}
