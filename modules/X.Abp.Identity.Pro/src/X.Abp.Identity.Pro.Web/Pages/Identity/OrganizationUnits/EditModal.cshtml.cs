// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Validation;

namespace X.Abp.Identity.Web.Pages.Identity.OrganizationUnits;

public class EditModalModel : IdentityPageModel
{
    [BindProperty]
    public OrganizationUnitInfoModel OrganizationUnit { get; set; }

    protected IOrganizationUnitAppService OrganizationUnitAppService { get; }

    public EditModalModel(IOrganizationUnitAppService organizationUnitAppService)
    {
        OrganizationUnitAppService = organizationUnitAppService;
    }

    public virtual async Task OnGetAsync(Guid id)
    {
        OrganizationUnit = ObjectMapper.Map<OrganizationUnitWithDetailsDto, OrganizationUnitInfoModel>(
            await OrganizationUnitAppService.GetAsync(id));
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        var input = ObjectMapper.Map<OrganizationUnitInfoModel, OrganizationUnitUpdateDto>(OrganizationUnit);
        await OrganizationUnitAppService.UpdateAsync(OrganizationUnit.Id, input);

        return NoContent();
    }

    public class OrganizationUnitInfoModel : ExtensibleObject, IHasConcurrencyStamp
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [Required]
        [DynamicStringLength(typeof(OrganizationUnitConsts), nameof(OrganizationUnitConsts.MaxDisplayNameLength))]
        public string DisplayName { get; set; }

        [HiddenInput]
        public string ConcurrencyStamp { get; set; }
    }
}
