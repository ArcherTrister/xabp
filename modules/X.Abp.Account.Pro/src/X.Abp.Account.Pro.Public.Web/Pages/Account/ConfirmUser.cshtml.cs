// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Settings;

using X.Abp.Identity;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class ConfirmUserModel : AccountPageModel
{
    public const string ConfirmUserScheme = "Abp.ConfirmUser";

    public UserInfoModel UserInfo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [Required]
    public string PhoneConfirmationToken { get; set; }

    protected IdentityUserManager UserManager { get; }

    public ConfirmUserModel(IdentityUserManager userManager)
    {
        UserManager = userManager;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        UserInfo = await RetrieveConfirmUser();

        return UserInfo == null
            ? await RedirectToLoginPageAsync()
            : UserInfo.TenantId != CurrentTenant.Id
            ? await RedirectToLoginPageAsync()
            : UserInfo.PhoneNumberConfirmed &&
            UserInfo.EmailConfirmed
            ? await RedirectToLoginPageAsync()
            : UserInfo.RequireConfirmedEmail &&
            UserInfo.EmailConfirmed &&
            !UserInfo.RequireConfirmedPhoneNumber
            ? await RedirectToLoginPageAsync()
            : UserInfo.RequireConfirmedPhoneNumber &&
            UserInfo.PhoneNumberConfirmed &&
            !UserInfo.RequireConfirmedEmail
            ? await RedirectToLoginPageAsync()
            : Page();
    }

    protected virtual async Task<IActionResult> RedirectToLoginPageAsync()
    {
        if (UserInfo != null)
        {
            // Try to cleanup confirm user id cookies
            await HttpContext.SignOutAsync(ConfirmUserScheme);
        }

        return RedirectToPage("./Login", new
        {
            ReturnUrl,
            ReturnUrlHash
        });
    }

    protected virtual async Task<UserInfoModel> RetrieveConfirmUser()
    {
        var result = await HttpContext.AuthenticateAsync(ConfirmUserScheme);
        if (result?.Principal != null)
        {
            var userId = result.Principal.FindUserId();
            if (userId == null)
            {
                return null;
            }

            var tenantId = result.Principal.FindTenantId();

            using (CurrentTenant.Change(tenantId))
            {
                var user = await UserManager.FindByIdAsync(userId.Value.ToString());
                return user == null
                    ? null
                    : new UserInfoModel
                    {
                        Id = user.Id,
                        TenantId = user.TenantId,

                        RequireConfirmedEmail = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail),
                        Email = user.Email,
                        EmailConfirmed = user.EmailConfirmed,

                        RequireConfirmedPhoneNumber = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber),
                        PhoneNumber = user.PhoneNumber,
                        PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    };
            }
        }

        return null;
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    public class UserInfoModel : IMultiTenant
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public bool RequireConfirmedEmail { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public bool RequireConfirmedPhoneNumber { get; set; }

        public string PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }
    }
}
