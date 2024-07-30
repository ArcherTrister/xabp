// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Identity;

namespace X.Abp.Identity.Web.Pages.Identity.Roles;

public class ClaimTypeEditModalModel : IdentityPageModel
{
    [BindProperty]
    public Guid RoleId { get; set; }

    public IdentityRoleDto Role { get; set; }

    [BindProperty]
    public ClaimsViewModel[] Claims { get; set; }

    protected IIdentityRoleAppService IdentityRoleAppService { get; }

    protected IIdentityClaimTypeAppService IdentityClaimTypeAppService { get; }

    public ClaimTypeEditModalModel(IIdentityRoleAppService identityRoleAppService,
        IIdentityClaimTypeAppService identityClaimTypeAppService)
    {
        IdentityRoleAppService = identityRoleAppService;
        IdentityClaimTypeAppService = identityClaimTypeAppService;
    }

    public virtual async Task OnGetAsync(Guid id)
    {
        RoleId = id;
        Role = await IdentityRoleAppService.GetAsync(RoleId);

        Claims = ObjectMapper.Map<List<ClaimTypeDto>, ClaimsViewModel[]>(await IdentityRoleAppService.GetAllClaimTypesAsync());
        var ownedClaims = await IdentityRoleAppService.GetClaimsAsync(RoleId);

        foreach (var claim in Claims)
        {
            var ownedClaimsFiltered = ownedClaims.Where(c => c.ClaimType == claim.Name).ToList();

            if (!ownedClaimsFiltered.Any())
            {
#pragma warning disable SA1122 // Use string.Empty for empty strings
                claim.Value.Add("");
#pragma warning restore SA1122 // Use string.Empty for empty strings
                continue;
            }

            foreach (var ownedClaim in ownedClaimsFiltered)
            {
                if (ownedClaim != null)
                {
                    claim.Value.Add(ownedClaim.ClaimValue);
                }
            }
        }
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var ownedClaims = new List<IdentityRoleClaimDto>();

        foreach (var claim in Claims)
        {
            foreach (var value in claim.Value)
            {
                if (!value.IsNullOrWhiteSpace())
                {
                    ownedClaims.Add(
                        new IdentityRoleClaimDto()
                        {
                            RoleId = RoleId,
                            ClaimType = claim.Name,
                            ClaimValue = value
                        });
                }
            }
        }

        await IdentityRoleAppService.UpdateClaimsAsync(RoleId, ownedClaims);

        return NoContent();
    }

    public class ClaimsViewModel
    {
        [Required]
        public string Name { get; set; }

        public bool Required { get; set; }

        public string Regex { get; set; }

        public string RegexDescription { get; set; }

        public IdentityClaimValueType ValueType { get; set; }

        public List<string> Value { get; set; } = new List<string>();
    }
}
