﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.Identity.Web.Pages.Identity.Roles;

public class CreateModalModel : IdentityPageModel
{
    [BindProperty]
    public RoleInfoModel Role { get; set; }

    protected IIdentityRoleAppService IdentityRoleAppService { get; }

    public CreateModalModel(IIdentityRoleAppService identityRoleAppService)
    {
        IdentityRoleAppService = identityRoleAppService;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
        Role = new RoleInfoModel();
        return Task.FromResult<IActionResult>(Page());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var input = ObjectMapper.Map<RoleInfoModel, IdentityRoleCreateDto>(Role);
        await IdentityRoleAppService.CreateAsync(input);

        return NoContent();
    }

    public class RoleInfoModel : ExtensibleObject
    {
        [Required]
        [DynamicStringLength(typeof(IdentityRoleConsts), nameof(IdentityRoleConsts.MaxNameLength))]
        [Display(Name = "DisplayName:RoleName")]
        public string Name { get; set; }

        [Display(Name = "DisplayName:IsDefault")]
        public bool IsDefault { get; set; }

        [Display(Name = "DisplayName:IsPublic")]
        public bool IsPublic { get; set; }
    }
}
