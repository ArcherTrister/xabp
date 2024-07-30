// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Saas;
using X.Abp.Saas.Dtos;
using X.Abp.Saas.Localization;

namespace X.Abp.Saas.Web.Pages.Saas.Tenants;

public class SetPasswordModel : SaasPageModel
{
    [BindProperty]
    public ChangeTenantPasswordViewModel ChangePasswordInput { get; set; }

    [BindProperty]
    public string TenantName { get; set; }

    public ITenantAppService TenantAppService { get; }

    public SetPasswordModel(ITenantAppService tenantAppService)
    {
        LocalizationResourceType = typeof(SaasResource);
        TenantAppService = tenantAppService;
    }

    public virtual async Task OnGetAsync(Guid id)
    {
        var saasTenantDto = await TenantAppService.GetAsync(id);
        TenantName = saasTenantDto.Name;
        ChangePasswordInput = new ChangeTenantPasswordViewModel()
        {
            Id = id
        };
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();
        await TenantAppService.SetPasswordAsync(ChangePasswordInput.Id, new SaasTenantSetPasswordDto()
        {
            Password = ChangePasswordInput.NewPassword,
            Username = ChangePasswordInput.UserName
        });
        return NoContent();
    }

    public class ChangeTenantPasswordViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string UserName { get; set; } = "admin";
    }
}
