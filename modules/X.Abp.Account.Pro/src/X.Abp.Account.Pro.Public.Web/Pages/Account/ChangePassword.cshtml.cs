// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Validation;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class ChangePasswordModel : AccountPageModel
{
    public static string ChangePasswordScheme = "Abp.ChangePassword";

    [BindProperty]
    public UserInfoModel UserInfo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool RememberMe { get; set; }

    public bool HideOldPasswordInput { get; set; }

    [DisableAuditing]
    [DynamicStringLength(typeof(IdentityUserConsts), "MaxPasswordLength", null)]
    [Required]
    [BindProperty]
    [DataType(DataType.Password)]
    public string CurrentPassword { get; set; }

    [DynamicStringLength(typeof(IdentityUserConsts), "MaxPasswordLength", null)]
    [DataType(DataType.Password)]
    [DisableAuditing]
    [BindProperty]
    [Required]
    public string NewPassword { get; set; }

    [DisableAuditing]
    [Required]
    [DataType(DataType.Password)]
    [Compare("NewPassword")]
    [DynamicStringLength(typeof(IdentityUserConsts), "MaxPasswordLength", null)]
    [BindProperty]
    public string NewPasswordConfirm { get; set; }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        UserInfo = await RetrieveChangePasswordUser();

        if (UserInfo == null || UserInfo.TenantId != CurrentTenant.Id)
        {
            await HttpContext.SignOutAsync(ChangePasswordScheme);
            return RedirectToPage("./Login", new { ReturnUrl, ReturnUrlHash });
        }

        HideOldPasswordInput = (await UserManager.GetByIdAsync(UserInfo.Id)).PasswordHash == null;
        return Page();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        if (CurrentPassword == NewPassword)
        {
            Alerts.Warning(L["NewPasswordSameAsOld"]);
            return Page();
        }

        UserInfoModel userInfo = await RetrieveChangePasswordUser();
        if (userInfo != null)
        {
            Guid? tenantId = userInfo.TenantId;
            Guid? id = CurrentTenant.Id;
            if (tenantId.HasValue == id.HasValue && (!tenantId.HasValue || tenantId.GetValueOrDefault() == id.GetValueOrDefault()))
            {
                try
                {
                    await IdentityOptions.SetAsync();
                    IdentityUser user = await UserManager.GetByIdAsync(userInfo.Id);
                    if (user.PasswordHash == null)
                    {
                        (await UserManager.AddPasswordAsync(user, NewPassword)).CheckIdentityErrors();
                    }
                    else
                    {
                        (await UserManager.ChangePasswordAsync(user, CurrentPassword, NewPassword)).CheckIdentityErrors();
                    }

                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = IdentitySecurityLogActionConsts.ChangePassword
                    });
                    user.SetShouldChangePasswordOnNextLogin(false);
                    (await UserManager.UpdateAsync(user)).CheckIdentityErrors();
                    await HttpContext.SignOutAsync(ChangePasswordScheme);
                    Microsoft.AspNetCore.Identity.SignInResult signInResult = await SignInManager.CallSignInOrTwoFactorAsync(user, RememberMe);
                    if (!signInResult.Succeeded)
                    {
                        if (signInResult.RequiresTwoFactor)
                        {
                            return RedirectToPage("./SendSecurityCode", new
                            {
                                returnUrl = ReturnUrl,
                                returnUrlHash = ReturnUrlHash
                            });
                        }
                        else
                        {
                            if (signInResult.IsLockedOut)
                            {
                                return RedirectToPage("./LockedOut", new
                                {
                                    returnUrl = ReturnUrl,
                                    returnUrlHash = ReturnUrlHash
                                });
                            }
                            else
                            {
                                return RedirectToPage("./Login", new
                                {
                                    ReturnUrl = ReturnUrl,
                                    ReturnUrlHash = ReturnUrlHash
                                });
                            }
                        }
                    }

                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                    {
                        Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                        Action = IdentitySecurityLogActionConsts.LoginSucceeded,
                        UserName = user.UserName
                    });
                    await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
                    return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
                }
                catch (Exception ex)
                {
                    Alerts.Warning(GetLocalizeExceptionMessage(ex));
                    return Page();
                }
            }
        }

        await HttpContext.SignOutAsync(ChangePasswordScheme);

        return RedirectToPage("./Login", new { ReturnUrl, ReturnUrlHash });
    }

    protected virtual async Task<UserInfoModel> RetrieveChangePasswordUser()
    {
        var authenticateResult = await HttpContext.AuthenticateAsync(ChangePasswordScheme);
        if (authenticateResult?.Principal == null)
        {
            return null;
        }

        var userId = authenticateResult.Principal.FindUserId();
        if (!userId.HasValue)
        {
            return null;
        }

        var tenantId = authenticateResult.Principal.FindTenantId();
        using (CurrentTenant.Change(tenantId, null))
        {
            var identityUser = await UserManager.FindByIdAsync(userId.Value.ToString());
            return identityUser == null
                ? null
                : new UserInfoModel()
                {
                    Id = identityUser.Id,
                    TenantId = identityUser.TenantId
                };
        }
    }

    public class UserInfoModel : IMultiTenant
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }
    }
}
