// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.IdentityServer.ApiScope;
using X.Abp.IdentityServer.ApiScope.Dtos;
using X.Abp.IdentityServer.ClaimType;
using X.Abp.IdentityServer.ClaimType.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiScopes
{
    public class EditModel : IdentityServerPageModel
    {
        protected IApiScopeAppService ApiScopeAppService { get; }

        protected IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

        [BindProperty]
        public ApiScopeUpdateModalView ApiScope { get; set; }

        public ApiScopePropertyModalView SampleProperty { get; set; } = new ApiScopePropertyModalView();

        public string[] AllClaims { get; set; }

        public EditModel(
          IApiScopeAppService apiScopeAppService,
          IIdentityServerClaimTypeAppService claimTypeAppService)
        {
            ApiScopeAppService = apiScopeAppService;
            ClaimTypeAppService = claimTypeAppService;
        }

        public virtual async Task OnGetAsync(Guid id)
        {
            ApiScopeWithDetailsDto source1 = await ApiScopeAppService.GetAsync(id);
            ApiScope = ObjectMapper.Map<ApiScopeWithDetailsDto, ApiScopeUpdateModalView>(source1);
            ApiScope.UserClaims = source1.UserClaims.Select(c => c.Type).ToArray();
            ApiScope.Properties = ApiScope.Properties.ToArray();
            List<IdentityClaimTypeDto> source2 = await ClaimTypeAppService.GetListAsync();
            AllClaims = source2.Select(ct => ct.Name).ToArray();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            UpdateApiScopeDto input = ObjectMapper.Map<ApiScopeUpdateModalView, UpdateApiScopeDto>(ApiScope);

            input.UserClaims = ApiScope.UserClaims.Where(c => !string.IsNullOrWhiteSpace(c)).Select(p => new ApiScopeClaimDto
            {
                ApiScopeId = ApiScope.Id,
                Type = p
            }).ToList();

            ApiScopePropertyDto[] properties = ApiScope.Properties;
            if (properties != null)
            {
                var scopePropertyDtoList = properties.Where(c =>
                {
                    if (c == null)
                    {
                        return false;
                    }

                    return !string.IsNullOrWhiteSpace(c.Key) || !string.IsNullOrWhiteSpace(c.Value);
                }).ToList();
                if (scopePropertyDtoList != null)
                {
                    input.Properties = scopePropertyDtoList;
                }
            }

            await ApiScopeAppService.UpdateAsync(ApiScope.Id, input);
            return NoContent();
        }
    }
}
