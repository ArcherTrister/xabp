// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;
using Volo.Abp.Users;

using X.Abp.Account.Dtos;

namespace X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;

public class ConfirmPhoneNumberModalModel : AccountPageModel
{
    [BindProperty]
    [Required]
    public string PhoneConfirmationToken { get; set; }

    public virtual async Task OnGetAsync()
    {
        if (!await SettingProvider.GetAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation, true))
        {
            // TODO: Localize!
            throw new BusinessException("Volo.Account:PhoneNumberConfirmationDisabled");
        }

        await AccountAppService.SendPhoneNumberConfirmationTokenAsync(new SendPhoneNumberConfirmationTokenDto
        {
            UserId = CurrentUser.GetId()
        });
    }

    public virtual async Task OnPostAsync()
    {
        await AccountAppService.ConfirmPhoneNumberAsync(new ConfirmPhoneNumberInput
        {
            UserId = CurrentUser.GetId(),
            Token = PhoneConfirmationToken
        });
    }
}
