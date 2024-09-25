// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp.Identity;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class LockedOut : AccountPageModel
{
    public static string LockedUserScheme = "Abp.LockedUser";

    public UserInfoModel UserInfo { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        UserInfo = await RetrieveConfirmUser();
        if (UserInfo == null)
        {
            return await RedirectToLoginPageAsync();
        }

        Guid? tenantId = UserInfo.TenantId;
        if (tenantId.HasValue != CurrentTenant.Id.HasValue || (tenantId.HasValue && (tenantId.GetValueOrDefault() != CurrentTenant.Id.GetValueOrDefault())))
        {
            return await RedirectToLoginPageAsync();
        }

        return UserInfo.LockoutEnabled && UserInfo.IsLocked ? Page() : await RedirectToLoginPageAsync();
    }

    public virtual Task<IActionResult> OnPostAsync()
    {
        return Task.FromResult<IActionResult>(Page());
    }

    protected virtual async Task<IActionResult> RedirectToLoginPageAsync()
    {
        if (UserInfo != null)
        {
            await HttpContext.SignOutAsync(LockedOut.LockedUserScheme);
        }

        return RedirectToPage("./Login", new
        {
            ReturnUrl,
            ReturnUrlHash
        });
    }

    protected virtual async Task<UserInfoModel> RetrieveConfirmUser()
    {
        AuthenticateResult authenticateResult = await HttpContext.AuthenticateAsync(LockedOut.LockedUserScheme);
        if (authenticateResult?.Principal == null)
        {
            return null;
        }

        Guid? userId = authenticateResult.Principal.FindUserId();
        if (!userId.HasValue)
        {
            return null;
        }

        Guid? tenantId = authenticateResult.Principal.FindTenantId();
        using (CurrentTenant.Change(tenantId))
        {
            IdentityUser identityUser = await UserManager.FindByIdAsync(userId.Value.ToString());
            if (identityUser == null)
            {
                return null;
            }

            return new UserInfoModel()
            {
                Id = identityUser.Id,
                TenantId = identityUser.TenantId,
                LockoutEnd = identityUser.LockoutEnd,
                LockoutEnabled = identityUser.LockoutEnabled
            };
        }
    }

    public class UserInfoModel
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public DateTimeOffset? LockoutEnd { get; set; }

        public bool LockoutEnabled { get; set; }

        public bool IsLocked => LockoutEnd.HasValue && LockoutEnd.Value > DateTimeOffset.Now;
    }
}
