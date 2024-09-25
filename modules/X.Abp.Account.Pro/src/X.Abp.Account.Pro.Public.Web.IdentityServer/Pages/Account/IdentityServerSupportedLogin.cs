// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.IdentityServer.AspNetIdentity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;

using X.Abp.Account.Dtos;
using X.Abp.Account.ExternalProviders;
using X.Abp.Account.Public.Web;
using X.Abp.Account.Public.Web.Pages.Account;
using X.Abp.Account.Security.Captcha;
using X.Abp.Account.Settings;
using X.Abp.Identity;

using static Volo.Abp.Identity.Settings.IdentitySettingNames;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Web.Pages.Account;

[ExposeServices(typeof(LoginModel))]
public class IdentityServerSupportedLoginModel : LoginModel
{
    protected IIdentityServerInteractionService Interaction { get; }

    protected IClientStore ClientStore { get; }

    protected IEventService IdentityServerEvents { get; }

    public IdentityServerSupportedLoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IAbpCaptchaValidatorFactory captchaValidatorFactory,
        IAccountExternalProviderAppService accountExternalProviderAppService,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        IIdentityServerInteractionService interaction,
        IClientStore clientStore,
        IEventService identityServerEvents)
        : base(
            schemeProvider,
            accountOptions,
            captchaValidatorFactory,
            accountExternalProviderAppService,
            currentPrincipalAccessor)
    {
        Interaction = interaction;
        ClientStore = clientStore;
        IdentityServerEvents = identityServerEvents;
    }

    public override async Task<IActionResult> OnGetAsync()
    {
        LoginInput = new LoginInputModel();

        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            return localLoginResult;
        }

        var context = await Interaction.GetAuthorizationContextAsync(ReturnUrl);

        if (context != null)
        {
            ShowCancelButton = true;

            LoginInput.UserNameOrEmailAddress = context.LoginHint;

            var tenant = context.Parameters[TenantResolverConsts.DefaultTenantKey];
            if (!string.IsNullOrEmpty(tenant))
            {
                CurrentTenant.Change(Guid.Parse(tenant));
                Response.Cookies.Append(TenantResolverConsts.DefaultTenantKey, tenant);
            }
        }

        if (context?.IdP != null)
        {
            LoginInput.UserNameOrEmailAddress = context.LoginHint;
            ExternalProviders = new[] { new ExternalProviderModel { AuthenticationScheme = context.IdP } };
            return Page();
        }

        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);
        if (context?.Client.ClientId != null)
        {
            var client = await ClientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
            if (client != null)
            {
                EnableLocalLogin = client.EnableLocalLogin;

                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Count != 0)
                {
                    ExternalProviders = ExternalProviders.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }
        }

        UseCaptcha = await UseCaptchaOnLoginAsync();

        IsLinkLogin = await VerifyLinkTokenAsync();
        if (IsLinkLogin)
        {
            if (CurrentUser.IsAuthenticated)
            {
                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = IdentitySecurityLogActionConsts.Logout
                });

                await SignInManager.SignOutAsync();

                return Redirect(HttpContext.Request.GetDisplayUrl());
            }
        }

        return Page();
    }

    [UnitOfWork]
    public override async Task<IActionResult> OnPostAsync(string action)
    {
        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            return localLoginResult;
        }

        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);

        var context = await Interaction.GetAuthorizationContextAsync(ReturnUrl);
        if (action == "Cancel")
        {
            if (context == null)
            {
                return Redirect("~/");
            }

            await Interaction.GrantConsentAsync(context, new ConsentResponse()
            {
                Error = AuthorizationError.AccessDenied
            });

            return Redirect(ReturnUrl);
        }

        try
        {
            await CaptchaVerificationAsync();
        }
        catch (UserFriendlyException e)
        {
            if (e is ScoreBelowThresholdException)
            {
                var onScoreBelowThresholdResult = await OnCaptchaScoreBelowThresholdAsync();
                if (onScoreBelowThresholdResult != null)
                {
                    return onScoreBelowThresholdResult;
                }
            }

            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }

        ValidateModel();

        await IdentityOptions.SetAsync();

        await ReplaceEmailToUsernameOfInputIfNeedsAsync();

        IsLinkLogin = await VerifyLinkTokenAsync();

        var result = await SignInManager.PasswordSignInAsync(
            LoginInput.UserNameOrEmailAddress,
            LoginInput.Password,
            LoginInput.RememberMe,
            true);

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = LoginInput.UserNameOrEmailAddress,
            ClientId = context?.Client?.ClientId
        });

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./SendSecurityCode", new
            {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash,
                rememberMe = LoginInput.RememberMe,
                linkUserId = LinkUserId,
                linkTenantId = LinkTenantId,
                linkToken = LinkToken
            });
        }

        if (result.IsLockedOut)
        {
            return RedirectToPage("./LockedOut", new
            {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
        }

        IdentityUser notAllowedUser;
        if (!result.IsNotAllowed)
        {
            if (!result.Succeeded)
            {
                Alerts.Danger(L["InvalidUserNameOrPassword"]);
                return Page();
            }

            IdentityUser user = await GetIdentityUserAsync(LoginInput.UserNameOrEmailAddress);
            await IdentityServerEvents.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id.ToString(), user.UserName, clientId: context?.Client.ClientId));
            if (!IsLinkLogin)
            {
                return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
            }

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
                    ClientId = context?.Client?.ClientId,
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
                    notAllowedUser = await UserManager.GetByIdAsync(LinkUserId.Value);
                    using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(notAllowedUser)))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = IdentityProSecurityLogActionConsts.LinkUser,
                            UserName = notAllowedUser.UserName,
                            ClientId = context?.Client?.ClientId,
                            ExtraProperties =
                                {
                                  {
                                    IdentityProSecurityLogActionConsts.LinkTargetTenantId,
                                    notAllowedUser.TenantId
                                  },
                                  {
                                    IdentityProSecurityLogActionConsts.LinkTargetUserId,
                                    notAllowedUser.Id
                                  }
                                }
                        });
                    }
                }

                return RedirectToPage("./LinkLogged", new
                {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash,
                    TargetLinkUserId = LinkUserId,
                    TargetLinkTenantId = LinkTenantId
                });
            }
        }
        else
        {
            notAllowedUser = await GetIdentityUserAsync(LoginInput.UserNameOrEmailAddress);
            if (notAllowedUser.ShouldChangePasswordOnNextLogin || await UserManager.ShouldPeriodicallyChangePasswordAsync(notAllowedUser))
            {
                await StoreChangePasswordUserAsync(notAllowedUser);
                return RedirectToPage("./ChangePassword", new
                {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash
                });
            }
            else
            {
                if (notAllowedUser.IsActive || await UserManager.CheckPasswordAsync(notAllowedUser, LoginInput.Password))
                {
                    await StoreConfirmUserAsync(notAllowedUser);
                    return RedirectToPage("./ConfirmUser", new
                    {
                        returnUrl = ReturnUrl,
                        returnUrlHash = ReturnUrlHash
                    });
                }
                else
                {
                    Alerts.Danger(L["LoginIsNotAllowed"]);
                    return Page();
                }
            }
        }
    }

    [UnitOfWork]
    public override async Task<IActionResult> OnPostExternalLoginAsync(string provider)
    {
        return AccountOptions.WindowsAuthenticationSchemeName == provider
            ? await ProcessWindowsLoginAsync()
            : await base.OnPostExternalLoginAsync(provider);
    }

    protected virtual async Task<IActionResult> ProcessWindowsLoginAsync()
    {
        var result = await HttpContext.AuthenticateAsync(AccountOptions.WindowsAuthenticationSchemeName);
        if (result.Succeeded)
        {
            var props = new AuthenticationProperties()
            {
                RedirectUri = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash }),
                Items =
                {
                    {
                        "LoginProvider", AccountOptions.WindowsAuthenticationSchemeName
                    },
                }
            };

            var claimsIdentity = new ClaimsIdentity(AccountOptions.WindowsAuthenticationSchemeName);
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, result.Principal.FindFirstValue(ClaimTypes.PrimarySid)));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, result.Principal.FindFirstValue(ClaimTypes.Name)));

            await HttpContext.SignInAsync(IdentityConstants.ExternalScheme, new ClaimsPrincipal(claimsIdentity), props);

            return Redirect(props.RedirectUri);
        }

        return Challenge(AccountOptions.WindowsAuthenticationSchemeName);
    }
}
