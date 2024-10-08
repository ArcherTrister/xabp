﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Validation;

namespace X.Abp.Identity.Web.Pages.Identity.Users;

public class SetPassword : IdentityPageModel
{
    [BindProperty]
    public ChangeUserPasswordViewModel ChangePasswordInput { get; set; }

    [BindProperty]
    public string UserName { get; set; }

    protected IIdentityUserAppService IdentityUserAppService { get; }

    public SetPassword(IIdentityUserAppService identityUserAppService)
    {
        IdentityUserAppService = identityUserAppService;
        LocalizationResourceType = typeof(IdentityResource);
    }

    public virtual async Task OnGetAsync(Guid id)
    {
        var user = await IdentityUserAppService.GetAsync(id);
        UserName = user.UserName;
        ChangePasswordInput = new ChangeUserPasswordViewModel { Id = id };
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        ValidateModel();

        await IdentityUserAppService.UpdatePasswordAsync(ChangePasswordInput.Id,
            new IdentityUserUpdatePasswordInput
            {
                NewPassword = ChangePasswordInput.NewPassword
            });

        return NoContent();
    }

    public class ChangeUserPasswordViewModel
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
