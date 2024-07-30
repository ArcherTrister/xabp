// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

using X.Abp.Account.Dtos;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class ForgotPasswordModel : AccountPageModel
{
    [Required]
    [EmailAddress]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
    [BindProperty]
    public string Email { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    public bool LocalLoginDisabled { get; set; }

    protected IAccountAppService AccountAppService { get; }

    public ForgotPasswordModel(IAccountAppService accountAppService)
    {
        AccountAppService = accountAppService;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            LocalLoginDisabled = true;
            return localLoginResult;
        }

        return Page();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            LocalLoginDisabled = true;
            return localLoginResult;
        }

        try
        {
            await AccountAppService.SendPasswordResetCodeAsync(
                new SendPasswordResetCodeDto
                {
                    Email = Email,
                    AppName = "MVC", // TODO: Const!
                    ReturnUrl = ReturnUrl,
                    ReturnUrlHash = ReturnUrlHash
                });
        }
        catch (BusinessException e)
        {
            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }

        return RedirectToPage(
            "./PasswordResetLinkSent",
            new
            {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
    }
}
