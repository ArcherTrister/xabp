// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.IdentityServer.ApiScope;
using X.Abp.IdentityServer.ApiScope.Dtos;
using X.Abp.IdentityServer.ClaimType;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiScopes
{
    public class CreateModel : IdentityServerPageModel
    {
        public IApiScopeAppService ApiScopeAppService { get; }

        public IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

        [BindProperty]
        public ApiScopeCreateModalView ApiScope { get; set; }

        public string[] AllClaims { get; set; }

        public CreateModel(
          IApiScopeAppService apiScopeAppService,
          IIdentityServerClaimTypeAppService claimTypeAppService)
        {
            ApiScopeAppService = apiScopeAppService;
            ClaimTypeAppService = claimTypeAppService;
        }

        public virtual async Task OnGetAsync()
        {
            ApiScope = new ApiScopeCreateModalView();
            AllClaims = (await ClaimTypeAppService.GetListAsync()).Select(ct => ct.Name).ToArray();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            CreateApiScopeDto input = ObjectMapper.Map<ApiScopeCreateModalView, CreateApiScopeDto>(ApiScope);
            input.UserClaims = ApiScope.UserClaims.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => new ApiScopeClaimDto()
            {
                Type = c
            }).ToList();
            input.Properties = new List<ApiScopePropertyDto>();

            await ApiScopeAppService.CreateAsync(input);
            return NoContent();
        }
    }
}
