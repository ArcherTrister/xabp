// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;

using X.Abp.Account.Dtos;
using X.Abp.Account.Settings;
using X.Abp.Identity;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class VerifySecurityCodeModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public bool RememberMe { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkTenantId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string LinkToken { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string Provider { get; set; }

    [BindProperty]
    public string Code { get; set; }

    [BindProperty]
    public bool RememberBrowser { get; set; }

    public bool IsRememberBrowserEnabled { get; protected set; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    public VerifySecurityCodeModel(ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        CurrentPrincipalAccessor = currentPrincipalAccessor;
    }

    [UnitOfWork]
    public virtual async Task OnGetAsync()
    {
        var user = await SignInManager.GetTwoFactorAuthenticationUserAsync() ?? throw new UserFriendlyException(L["VerifySecurityCodeNotLoggedInErrorMessage"]);

        // TODO: CheckCurrentTenant(await SignInManager.GetVerifiedTenantIdAsync());
        CheckCurrentTenant(user.TenantId);

        IsRememberBrowserEnabled = await IsRememberBrowserEnabledAsync();
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync()
    {
        await IdentityOptions.SetAsync();

        Microsoft.AspNetCore.Identity.SignInResult signInResult = await SignInManager.TwoFactorSignInAsync(Provider, Code, RememberMe, await IsRememberBrowserEnabledAsync() && RememberBrowser);

        IdentityUser user;
        if (!signInResult.Succeeded)
        {
            if (signInResult.IsLockedOut)
            {
                return RedirectToPage("./LockedOut", new
                {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash
                });
            }

            if (!signInResult.IsNotAllowed)
            {
                Alerts.Warning(L["LoginIsNotAllowed"]);
                return Page();
            }

            user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
            if (user != null)
            {
                bool shouldChangePasswordOnNextLogin;
                if (!(shouldChangePasswordOnNextLogin = user.ShouldChangePasswordOnNextLogin))
                {
                    shouldChangePasswordOnNextLogin = await UserManager.ShouldPeriodicallyChangePasswordAsync(user);
                }

                if (!shouldChangePasswordOnNextLogin)
                {
                    if (user.IsActive)
                    {
                        await StoreConfirmUserAsync(user);

                        return RedirectToPage("./ConfirmUser", new
                        {
                            returnUrl = ReturnUrl,
                            returnUrlHash = ReturnUrlHash
                        });
                    }
                }
                else
                {
                    await StoreChangePasswordUserAsync(user);

                    return RedirectToPage("./ChangePassword", new
                    {
                        returnUrl = ReturnUrl,
                        returnUrlHash = ReturnUrlHash
                    });
                }
            }

            Alerts.Warning(L["InvalidSecurityCode"]);
            return Page();
        }

        user = await SignInManager.GetTwoFactorAuthenticationUserAsync();
        if (await VerifyLinkTokenAsync())
        {
            using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(user)))
            {
                await IdentityLinkUserAppService.LinkAsync(new LinkUserInput
                {
                    UserId = LinkUserId.Value,
                    TenantId = LinkTenantId,
                    Token = LinkToken
                });

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = IdentityProSecurityLogActionConsts.LinkUser,
                    UserName = user.UserName,
                    ExtraProperties =
                    {
                      {
                         IdentityProSecurityLogActionConsts.LinkTargetTenantId,
                         LinkTenantId
                      },
                      {
                         IdentityProSecurityLogActionConsts.LinkTargetUserId,
                         LinkUserId
                      }
                    }
                });
                using (CurrentTenant.Change(LinkTenantId))
                {
                    IdentityUser targetUser = await UserManager.GetByIdAsync(LinkUserId.Value);
                    using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(targetUser)))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = IdentityProSecurityLogActionConsts.LinkUser,
                            UserName = targetUser.UserName,
                            ExtraProperties =
                            {
                              {
                                 IdentityProSecurityLogActionConsts.LinkTargetTenantId,
                                 targetUser.TenantId
                              },
                              {
                                 IdentityProSecurityLogActionConsts.LinkTargetUserId,
                                 targetUser.Id
                              }
                            }
                        });
                    }
                }
            }
        }

        await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
        return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
    }

    protected virtual async Task<bool> VerifyLinkTokenAsync()
    {
        return !LinkToken.IsNullOrWhiteSpace() && LinkUserId != null
            && await IdentityLinkUserAppService.VerifyLinkTokenAsync(new VerifyLinkTokenInput
            {
                UserId = LinkUserId.Value,
                TenantId = LinkTenantId,
                Token = LinkToken
            });
    }

    protected virtual async Task<bool> IsRememberBrowserEnabledAsync()
    {
        return await SettingProvider.IsTrueAsync(AccountSettingNames.TwoFactorLogin.IsRememberBrowserEnabled);
    }
}
