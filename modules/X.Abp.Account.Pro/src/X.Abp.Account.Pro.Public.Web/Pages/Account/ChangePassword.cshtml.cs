// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
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

    protected SignInManager<IdentityUser> SignInManager { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public ChangePasswordModel(
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions> identityOptions)
    {
        SignInManager = signInManager;
        UserManager = userManager;
        IdentitySecurityLogManager = identitySecurityLogManager;
        IdentityOptions = identityOptions;
    }

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
        var userInfo = await RetrieveChangePasswordUser();
        if (userInfo != null && !(userInfo.TenantId != CurrentTenant.Id))
        {
            try
            {
                await IdentityOptions.SetAsync();
                var user = await UserManager.GetByIdAsync(userInfo.Id);
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
                await SignInManager.SignInAsync(user, RememberMe);
                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                {
                    Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                    Action = IdentitySecurityLogActionConsts.LoginSucceeded,
                    UserName = user.UserName
                });
                return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
            }
            catch (Exception ex)
            {
                Alerts.Warning(GetLocalizeExceptionMessage(ex));
                return Page();
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
