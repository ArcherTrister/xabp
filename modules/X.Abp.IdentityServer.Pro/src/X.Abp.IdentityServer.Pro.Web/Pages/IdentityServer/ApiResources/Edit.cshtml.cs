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
using X.Abp.IdentityServer.ApiScope;
using X.Abp.IdentityServer.ClaimType;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiResources
{
    public class EditModel : IdentityServerPageModel
    {
        protected IApiResourceAppService ApiResourceAppService { get; }

        protected IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

        public IApiScopeAppService ApiScopeAppService { get; }

        [BindProperty]
        public ApiResourceUpdateModalView ApiResource { get; set; }

        public string[] AllClaims { get; set; }

        public ApiResourceSecretModalView SampleSecret { get; set; } = new ApiResourceSecretModalView();

        public ApiResourcePropertyModalView SampleProperty { get; set; } = new ApiResourcePropertyModalView();

        public List<SelectListItem> ScopeNameList { get; set; } = new List<SelectListItem>();

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

        public EditModel(
          IApiResourceAppService apiResourceAppService,
          IIdentityServerClaimTypeAppService claimTypeAppService,
          IApiScopeAppService apiScopeAppService)
        {
            ApiResourceAppService = apiResourceAppService;
            ClaimTypeAppService = claimTypeAppService;
            ApiScopeAppService = apiScopeAppService;
        }

        public virtual async Task OnGetAsync(Guid id)
        {
            ApiResourceWithDetailsDto apiResource = await ApiResourceAppService.GetAsync(id);
            ApiResource = ObjectMapper.Map<ApiResourceWithDetailsDto, ApiResourceUpdateModalView>(apiResource);
            ApiResource.UserClaims = apiResource.UserClaims.Select(c => c.Type).ToArray();
            ApiResource.Secrets = apiResource.Secrets.ToArray();
            ApiResource.Properties = apiResource.Properties.ToArray();

            AllClaims = (await ClaimTypeAppService.GetListAsync()).Select(ct => ct.Name).ToArray();

            ScopeNameList = (await ApiScopeAppService.GetAllListAsync()).Select(s => new SelectListItem(s.DisplayName, s.Name, apiResource.Scopes.Any(apiScope => apiScope.Scope == s.Name))).ToList();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            UpdateApiResourceDto updateApiResource = ObjectMapper.Map<ApiResourceUpdateModalView, UpdateApiResourceDto>(ApiResource);

            updateApiResource.UserClaims = ApiResource.UserClaims.Where(c => !string.IsNullOrWhiteSpace(c)).Select(p => new ApiResourceClaimDto
            {
                ApiResourceId = ApiResource.Id,
                Type = p
            }).ToList();

            List<ApiResourceSecretDto> secrets = updateApiResource.Secrets;
            if (secrets != null)
            {
                var resourceSecretDtoList = secrets.Where(c => c != null && !string.IsNullOrWhiteSpace(c.Value)).ToList();
                if (resourceSecretDtoList != null)
                {
                    updateApiResource.Secrets = resourceSecretDtoList;
                }
            }

            ApiResourcePropertyDto[] properties = ApiResource.Properties;
            if (properties != null)
            {
                var resourcePropertyDtoList = properties.Where(c =>
                {
                    if (c == null)
                    {
                        return false;
                    }

                    return !string.IsNullOrWhiteSpace(c.Key) || !string.IsNullOrWhiteSpace(c.Value);
                }).ToList();

                if (resourcePropertyDtoList != null)
                {
                    updateApiResource.Properties = resourcePropertyDtoList;
                }
            }

            List<string> scopes = ApiResource.Scopes;
            if (scopes != null)
            {
                var resourceScopeDtoList = scopes.Select(p => new ApiResourceScopeDto { ApiResourceId = ApiResource.Id, Scope = p }).ToList();
                if (resourceScopeDtoList != null)
                {
                    updateApiResource.Scopes = resourceScopeDtoList;
                }
            }

            await ApiResourceAppService.UpdateAsync(ApiResource.Id, updateApiResource);
            return NoContent();
        }
    }
}
