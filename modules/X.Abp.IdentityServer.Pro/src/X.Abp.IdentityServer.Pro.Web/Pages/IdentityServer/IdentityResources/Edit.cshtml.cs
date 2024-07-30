// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.IdentityServer.ClaimType;
using X.Abp.IdentityServer.ClaimType.Dtos;
using X.Abp.IdentityServer.IdentityResource;
using X.Abp.IdentityServer.IdentityResource.Dtos;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer.IdentityResources
{
    public class EditModel : IdentityServerPageModel
    {
        protected IIdentityResourceAppService IdentityResourceAppService { get; }

        protected IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

        [BindProperty]
        public IdentityResourceUpdateModalView IdentityResource { get; set; }

        public IdentityResourcePropertyModalView SampleProperty { get; set; } = new IdentityResourcePropertyModalView();

        public string[] AllClaims { get; set; }

        public EditModel(
          IIdentityResourceAppService identityResourceAppService,
          IIdentityServerClaimTypeAppService claimTypeAppService)
        {
            IdentityResourceAppService = identityResourceAppService;
            ClaimTypeAppService = claimTypeAppService;
        }

        public virtual async Task OnGetAsync(Guid id)
        {
            IdentityResourceWithDetailsDto source1 = await IdentityResourceAppService.GetAsync(id);
            IdentityResource = ObjectMapper.Map<IdentityResourceWithDetailsDto, IdentityResourceUpdateModalView>(source1);
            IdentityResource.UserClaims = source1.UserClaims.Select(c => c.Type).ToArray();
            IdentityResource.Properties = source1.Properties.ToArray();
            List<IdentityClaimTypeDto> source2 = await ClaimTypeAppService.GetListAsync();
            AllClaims = source2.Select(ct => ct.Name).ToArray();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            UpdateIdentityResourceDto input = ObjectMapper.Map<IdentityResourceUpdateModalView, UpdateIdentityResourceDto>(IdentityResource);
            input.UserClaims = IdentityResource.UserClaims.Where(c => !string.IsNullOrWhiteSpace(c)).Select(c => new IdentityResourceClaimDto()
            {
                IdentityResourceId = IdentityResource.Id,
                Type = c
            }).ToList();

            IdentityResourcePropertyDto[] properties = IdentityResource.Properties;
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
                    input.Properties = resourcePropertyDtoList;
                }
            }

            await IdentityResourceAppService.UpdateAsync(IdentityResource.Id, input);
            return NoContent();
        }
    }
}
