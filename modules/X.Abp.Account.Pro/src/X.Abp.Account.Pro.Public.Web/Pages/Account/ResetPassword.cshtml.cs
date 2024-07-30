// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Validation;

using X.Abp.Account.Dtos;

namespace X.Abp.Account.Public.Web.Pages.Account;

// TODO: Implement live password complexity check on the razor view!
public class ResetPasswordModel : AccountPageModel
{
    [Required]
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid UserId { get; set; }

    [Required]
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ResetToken { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DisableAuditing]
    public string Password { get; set; }

    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
    [DisableAuditing]
    public string ConfirmPassword { get; set; }

    public string NormalizeReturnUrl { get; set; }

    public bool LocalLoginDisabled { get; set; }

    protected IAccountAppService AccountAppService { get; }

    public ResetPasswordModel(IAccountAppService accountAppService)
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

        await SetNormalizeReturnUrl();
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
            ValidateModel();
            await SetNormalizeReturnUrl();

            await AccountAppService.ResetPasswordAsync(
                new ResetPasswordDto
                {
                    UserId = UserId,
                    ResetToken = ResetToken,
                    Password = Password
                });
        }
        catch (Exception e)
        {
            // TODO:XAbpIdentityResultException
            if (e is AbpIdentityResultException && !string.IsNullOrWhiteSpace(e.Message))
            {
                Alerts.Warning(GetLocalizeExceptionMessage(e));
                return Page();
            }

            if (e is AbpValidationException)
            {
                return Page();
            }

            throw;
        }

        // TODO: Try to automatically login!
        return RedirectToPage("./ResetPasswordConfirmation", new
        {
            returnUrl = NormalizeReturnUrl
        });
    }

    protected override void ValidateModel()
    {
        if (!Equals(Password, ConfirmPassword))
        {
            ModelState.AddModelError("ConfirmPassword",
                L["'{0}' and '{1}' do not match.", "ConfirmPassword", "Password"]);
        }

        base.ValidateModel();
    }

    private async Task SetNormalizeReturnUrl()
    {
        NormalizeReturnUrl = await GetRedirectUrlAsync(ReturnUrl, ReturnUrlHash);
    }
}
