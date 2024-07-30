// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.IdentityServer.ApiResource;
using X.Abp.IdentityServer.ApiResource.Dtos;
using X.Abp.IdentityServer.ClaimType;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.ApiResources
{
    public class CreateModel : IdentityServerPageModel
    {
        protected IApiResourceAppService ApiResourceAppService { get; }

        protected IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

        [BindProperty]
        public ApiResourceCreateModalView ApiResource { get; set; }

        public string[] AllClaims { get; set; }

        public CreateModel(
          IApiResourceAppService apiResourceAppService,
          IIdentityServerClaimTypeAppService claimTypeAppService)
        {
            ApiResourceAppService = apiResourceAppService;
            ClaimTypeAppService = claimTypeAppService;
        }

        public virtual async Task OnGetAsync()
        {
            ApiResource = new ApiResourceCreateModalView();
            AllClaims = (await ClaimTypeAppService.GetListAsync()).Select(ct => ct.Name).ToArray();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            CreateApiResourceDto createApiResource = ObjectMapper.Map<ApiResourceCreateModalView, CreateApiResourceDto>(ApiResource);
            createApiResource.UserClaims = ApiResource.UserClaims.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => new ApiResourceClaimDto()
            {
                Type = c
            }).ToList();
            await ApiResourceAppService.CreateAsync(createApiResource);
            return NoContent();
        }
    }
}
