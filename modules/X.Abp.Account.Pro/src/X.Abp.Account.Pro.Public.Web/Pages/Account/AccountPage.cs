// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.AspNetCore.ExceptionHandling;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.AspNetCore.WebClientInfo;
using Volo.Abp.Clients;
using Volo.Abp.ExceptionHandling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;

using X.Abp.Account.Localization;
using X.Abp.Account.Settings;
using X.Abp.Identity;
using X.Captcha;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

public abstract class AccountPageModel : AbpPageModel
{
    public IAccountAppService AccountAppService => LazyServiceProvider.LazyGetRequiredService<IAccountAppService>();

    public AbpSignInManager SignInManager => LazyServiceProvider.LazyGetRequiredService<AbpSignInManager>();

    public IdentitySessionManager IdentitySessionManager => LazyServiceProvider.LazyGetRequiredService<IdentitySessionManager>();

    public ICurrentClient CurrentClient => LazyServiceProvider.LazyGetRequiredService<ICurrentClient>();

    public IWebClientInfoProvider WebClientInfoProvider => LazyServiceProvider.LazyGetRequiredService<IWebClientInfoProvider>();

    public IdentityUserManager UserManager => LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();

    public IdentitySecurityLogManager IdentitySecurityLogManager => LazyServiceProvider.LazyGetRequiredService<IdentitySecurityLogManager>();

    public IIdentityLinkUserAppService IdentityLinkUserAppService => LazyServiceProvider.LazyGetRequiredService<IIdentityLinkUserAppService>();

    public IOptions<IdentityOptions> IdentityOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<IdentityOptions>>();

    public IOptionsSnapshot<CaptchaOptions> CaptchaOptions => LazyServiceProvider.LazyGetRequiredService<IOptionsSnapshot<CaptchaOptions>>();

    public IExceptionToErrorInfoConverter ExceptionToErrorInfoConverter => LazyServiceProvider.LazyGetRequiredService<IExceptionToErrorInfoConverter>();

    public IdentityDynamicClaimsPrincipalContributorCache IdentityDynamicClaimsPrincipalContributorCache => LazyServiceProvider.LazyGetRequiredService<IdentityDynamicClaimsPrincipalContributorCache>();

    protected AccountPageModel()
    {
        ObjectMapperContext = typeof(AbpAccountPublicWebModule);
        LocalizationResourceType = typeof(AccountResource);
    }

    protected virtual void CheckCurrentTenant(Guid? tenantId)
    {
        if (CurrentTenant.Id != tenantId)
        {
            throw new ApplicationException(
                $"Current tenant is different than given tenant. CurrentTenant.Id: {CurrentTenant.Id}, given tenantId: {tenantId}");
        }
    }

    protected virtual void CheckIdentityErrors(IdentityResult identityResult)
    {
        if (!identityResult.Succeeded)
        {
            throw new UserFriendlyException("Operation failed: " + identityResult.Errors.Select(e => $"[{e.Code}] {e.Description}").JoinAsString(", "));
        }

        // identityResult.CheckIdentityErrors(LocalizationManager);
    }

    protected virtual string GetLocalizeExceptionMessage(Exception exception)
    {
        return exception is ILocalizeErrorMessage or IHasErrorCode
            ? ExceptionToErrorInfoConverter.Convert(exception).Message
            : exception.Message;
    }

    protected virtual async Task StoreConfirmUserAsync(IdentityUser user)
    {
        var identity = new ClaimsIdentity(ConfirmUserModel.ConfirmUserScheme);
        identity.AddClaim(new Claim(AbpClaimTypes.UserId, user.Id.ToString()));

        // identity.AddIfNotContains(new Claim(ClaimTypes.Name, user.Id.ToString()));
        if (user.TenantId.HasValue)
        {
            identity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
        }

        await HttpContext.SignInAsync(ConfirmUserModel.ConfirmUserScheme, new ClaimsPrincipal(identity));
    }

    protected virtual async Task StoreChangePasswordUserAsync(IdentityUser user)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(ChangePasswordModel.ChangePasswordScheme);
        claimsIdentity.AddClaim(new Claim(AbpClaimTypes.UserId, user.Id.ToString()));
        if (user.TenantId.HasValue)
        {
            claimsIdentity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
        }

        await HttpContext.SignInAsync(ChangePasswordModel.ChangePasswordScheme, new ClaimsPrincipal(claimsIdentity));
    }

    protected virtual async Task StoreLockedUserAsync(IdentityUser user)
    {
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(LockedOut.LockedUserScheme);
        claimsIdentity.AddClaim(new Claim(AbpClaimTypes.UserId, user.Id.ToString()));
        if (user.TenantId.HasValue)
        {
            claimsIdentity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
        }

        await HttpContext.SignInAsync(LockedOut.LockedUserScheme, new ClaimsPrincipal(claimsIdentity));
    }

    protected virtual async Task<IActionResult> CheckLocalLoginAsync()
    {
        var enableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);
        if (!enableLocalLogin)
        {
            Alerts.Warning(L["LocalLoginIsNotEnabled"]);
            return Page();
        }

        return null;
    }
}
