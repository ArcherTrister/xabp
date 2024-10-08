﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.Identity.Web.Pages.Identity.OrganizationUnits;

public class CreateModalModel : IdentityPageModel
{
    [BindProperty]
    public OrganizationUnitInfoModel OrganizationUnit { get; set; }

    protected IOrganizationUnitAppService OrganizationUnitAppService { get; }

    public CreateModalModel(IOrganizationUnitAppService organizationUnitAppService)
    {
        OrganizationUnitAppService = organizationUnitAppService;
        OrganizationUnit = new OrganizationUnitInfoModel();
    }

    public virtual Task OnGetAsync(Guid? parentId)
    {
        OrganizationUnit.ParentId = parentId;
        return Task.CompletedTask;
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var input = ObjectMapper.Map<OrganizationUnitInfoModel, OrganizationUnitCreateDto>(OrganizationUnit);
        await OrganizationUnitAppService.CreateAsync(input);

        return NoContent();
    }

    public class OrganizationUnitInfoModel : ExtensibleObject
    {
        [HiddenInput]
        public Guid? ParentId { get; set; }

        [Required]
        [DynamicStringLength(typeof(OrganizationUnitConsts), nameof(OrganizationUnitConsts.MaxDisplayNameLength))]
        public string DisplayName { get; set; }
    }
}
