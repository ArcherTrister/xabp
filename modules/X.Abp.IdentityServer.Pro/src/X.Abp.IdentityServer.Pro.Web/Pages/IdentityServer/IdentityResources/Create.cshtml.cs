// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.IdentityServer.ClaimType;
using X.Abp.IdentityServer.IdentityResource;
using X.Abp.IdentityServer.IdentityResource.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.IdentityResources
{
    public class CreateModel : IdentityServerPageModel
    {
        protected IIdentityResourceAppService IdentityResourceAppService { get; }

        protected IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

        [BindProperty]
        public IdentityResourceCreateModalView IdentityResource { get; set; }

        public string[] AllClaims { get; set; }

        public CreateModel(
          IIdentityResourceAppService identityResourceAppService,
          IIdentityServerClaimTypeAppService claimTypeAppService)
        {
            IdentityResourceAppService = identityResourceAppService;
            ClaimTypeAppService = claimTypeAppService;
        }

        public virtual async Task OnGetAsync()
        {
            IdentityResource = new IdentityResourceCreateModalView();
            AllClaims = (await ClaimTypeAppService.GetListAsync()).Select(ct => ct.Name).ToArray();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            CreateIdentityResourceDto input = ObjectMapper.Map<IdentityResourceCreateModalView, CreateIdentityResourceDto>(IdentityResource);
            input.UserClaims = IdentityResource.UserClaims.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => new IdentityResourceClaimDto()
            {
                Type = c
            }).ToList();
            input.Properties = new List<IdentityResourcePropertyDto>();

            await IdentityResourceAppService.CreateAsync(input);
            return NoContent();
        }
    }
}
