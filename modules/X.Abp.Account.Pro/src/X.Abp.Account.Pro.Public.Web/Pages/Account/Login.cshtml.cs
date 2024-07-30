// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Reflection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Validation;

using X.Abp.Account.Dtos;
using X.Abp.Account.ExternalProviders;
using X.Abp.Account.Public.Web.Security.Captcha;
using X.Abp.Account.Public.Web.Security.Claims;
using X.Abp.Account.Security.Captcha;
using X.Abp.Account.Settings;
using X.Abp.Identity;
using X.Captcha;

using IdentityUser = Volo.Abp.Identity.IdentityUser;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace X.Abp.Account.Public.Web.Pages.Account;

[DisableAuditing]
public class LoginModel : AccountPageModel
{
    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkUserId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid? LinkTenantId { get; set; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public string LinkToken { get; set; }

    public bool IsLinkLogin { get; set; }

    [BindProperty]
    public LoginInputModel LoginInput { get; set; }

    public bool EnableLocalLogin { get; set; }

    public bool IsSelfRegistrationEnabled { get; set; }

    public bool ShowCancelButton { get; set; }

    public bool UseCaptcha { get; set; }

    // TODO: Why there is an ExternalProviders if only the VisibleExternalProviders is used.
    public IEnumerable<ExternalProviderModel> ExternalProviders { get; set; }

    public IEnumerable<ExternalProviderModel> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

    public bool IsExternalLoginOnly => EnableLocalLogin == false && ExternalProviders?.Count() == 1;

    public string ExternalLoginScheme => IsExternalLoginOnly ? ExternalProviders?.SingleOrDefault()?.AuthenticationScheme : null;

    protected IAuthenticationSchemeProvider SchemeProvider { get; }

    protected AbpAccountOptions AccountOptions { get; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    protected IAbpCaptchaValidatorFactory CaptchaValidatorFactory { get; }

    protected IAccountExternalProviderAppService AccountExternalProviderAppService { get; }

    protected SignInManager<IdentityUser> SignInManager { get; }

    protected IdentityUserManager UserManager { get; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }

    protected IIdentityLinkUserAppService IdentityLinkUserAppService { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    public IOptionsSnapshot<CaptchaOptions> CaptchaOptions { get; }

    public LoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IAbpCaptchaValidatorFactory captchaValidatorFactory,
        IAccountExternalProviderAppService accountExternalProviderAppService,
        ICurrentPrincipalAccessor currentPrincipalAccessor,
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IdentitySecurityLogManager identitySecurityLogManager,
        IIdentityLinkUserAppService identityLinkUserAppService,
        IOptions<IdentityOptions> identityOptions,
        IOptionsSnapshot<CaptchaOptions> captchaOptions)
    {
        SchemeProvider = schemeProvider;
        AccountOptions = accountOptions.Value;
        CaptchaValidatorFactory = captchaValidatorFactory;
        AccountExternalProviderAppService = accountExternalProviderAppService;
        CurrentPrincipalAccessor = currentPrincipalAccessor;
        SignInManager = signInManager;
        UserManager = userManager;
        IdentitySecurityLogManager = identitySecurityLogManager;
        IdentityLinkUserAppService = identityLinkUserAppService;
        IdentityOptions = identityOptions;
        CaptchaOptions = captchaOptions;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        LoginInput = new LoginInputModel();

        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            return localLoginResult;
        }

        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);

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

    // TODO: Will be removed when we implement action filter
    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync(string action)
    {
        try
        {
            await CaptchaVerificationAsync();
        }
        catch (UserFriendlyException e)
        {
            if (e is ScoreBelowThresholdException)
            {
                var onScoreBelowThresholdResult = OnCaptchaScoreBelowThreshold();
                if (onScoreBelowThresholdResult != null)
                {
                    return await onScoreBelowThresholdResult;
                }
            }

            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }

        ValidateModel();

        await IdentityOptions.SetAsync();

        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            return localLoginResult;
        }

        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);

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
            UserName = LoginInput.UserNameOrEmailAddress
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

        if (result.IsNotAllowed)
        {
            var notAllowedUser = await GetIdentityUserAsync(LoginInput.UserNameOrEmailAddress);
            if (notAllowedUser.IsActive && await UserManager.CheckPasswordAsync(notAllowedUser, LoginInput.Password))
            {
                await StoreConfirmUserAsync(notAllowedUser);
                return RedirectToPage("./ConfirmUser", new
                {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash
                });
            }

            Alerts.Danger(L["LoginIsNotAllowed"]);
            return Page();
        }

        if (!result.Succeeded)
        {
            Alerts.Danger(L["InvalidUserNameOrPassword"]);
            return Page();
        }

        var user = await GetIdentityUserAsync(LoginInput.UserNameOrEmailAddress);

        if (IsLinkLogin)
        {
            using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(user)))
            {
                await IdentityLinkUserAppService.LinkAsync(new LinkUserInput
                {
                    UserId = LinkUserId.Value,
                    TenantId = LinkTenantId,
                    Token = LinkToken
                });

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = IdentityProSecurityLogActionConsts.LinkUser,
                    UserName = user.UserName,
                    ExtraProperties =
                    {
                        { IdentityProSecurityLogActionConsts.LinkTargetTenantId, LinkTenantId },
                        { IdentityProSecurityLogActionConsts.LinkTargetUserId, LinkUserId }
                    }
                });

                using (CurrentTenant.Change(LinkTenantId))
                {
                    var targetUser = await UserManager.GetByIdAsync(LinkUserId.Value);
                    using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(targetUser)))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = IdentityProSecurityLogActionConsts.LinkUser,
                            UserName = targetUser.UserName,
                            ExtraProperties =
                            {
                                { IdentityProSecurityLogActionConsts.LinkTargetTenantId, targetUser.TenantId },
                                { IdentityProSecurityLogActionConsts.LinkTargetUserId, targetUser.Id }
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

        return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
    }

    protected virtual async Task<IdentityUser> GetIdentityUserAsync(string userNameOrEmailAddress)
    {
        // TODO: Find a way of getting user's id from the logged in user and do not query it again like that!
        var user = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
            await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
        Debug.Assert(user != null, nameof(user) + " != null");

        return user;
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetCreateLinkUserAsync()
    {
        IsLinkLogin = await VerifyLinkTokenAsync();
        if (IsLinkLogin)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.Logout
            });

            await SignInManager.SignOutAsync();
        }

        return RedirectToPage("./Login", new
        {
            ReturnUrl,
            ReturnUrlHash,
            LinkUserId,
            LinkTenantId,
            LinkToken
        });
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

    protected virtual async Task<List<ExternalProviderModel>> GetExternalProvidersAsync()
    {
        var schemes = await SchemeProvider.GetAllSchemesAsync();
        var externalProviders = await AccountExternalProviderAppService.GetAllAsync();

        var externalProviderModels = new List<ExternalProviderModel>();
        foreach (var scheme in schemes)
        {
            if (IsRemoteAuthenticationHandler(scheme, externalProviders) || scheme.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
            {
                externalProviderModels.Add(new ExternalProviderModel
                {
                    DisplayName = scheme.DisplayName,
                    AuthenticationScheme = scheme.Name,
                    Icon = AccountOptions.ExternalProviderIconMap.GetOrDefault(scheme.Name)
                });
            }
        }

        return externalProviderModels;
    }

    protected virtual bool IsRemoteAuthenticationHandler(AuthenticationScheme scheme, ExternalProviderDto externalProviders)
    {
        if (ReflectionHelper.IsAssignableToGenericType(scheme.HandlerType, typeof(RemoteAuthenticationHandler<>)))
        {
            var provider = externalProviders.Providers.FirstOrDefault(x => x.Name == scheme.Name);
            return provider == null || provider.Enabled;
        }

        return false;
    }

    protected virtual async Task ReplaceEmailToUsernameOfInputIfNeedsAsync()
    {
        if (!ValidationHelper.IsValidEmailAddress(LoginInput.UserNameOrEmailAddress))
        {
            return;
        }

        var userByUsername = await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress);
        if (userByUsername != null)
        {
            return;
        }

        var userByEmail = await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
        if (userByEmail == null)
        {
            return;
        }

        LoginInput.UserNameOrEmailAddress = userByEmail.UserName;
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostExternalLoginAsync(string provider)
    {
        var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash });
        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetExternalLoginCallbackAsync(string returnUrl = "", string returnUrlHash = "", string remoteError = null)
    {
        // TODO: Did not implemented Identity Server 4 sample for this method (see ExternalLoginCallback in Quickstart of IDS4 sample)
        /* Also did not implement these:
         * - Logout(string logoutId)
         */

        if (remoteError != null)
        {
            Logger.LogWarning($"External login callback error: {remoteError}");
            return RedirectToPage("./Login");
        }

        await IdentityOptions.SetAsync();

        var loginInfo = await SignInManager.GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            Logger.LogWarning("External login info is not available");
            return RedirectToPage("./Login");
        }

        IsLinkLogin = await VerifyLinkTokenAsync();

        var user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);

        var result = await ExternalLoginSignInAsync(user, loginInfo.LoginProvider, loginInfo.ProviderKey, true, false);

        if (!result.Succeeded)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                Action = "Login" + result
            });
        }

        if (result.IsLockedOut)
        {
            Logger.LogWarning($"Cannot proceed because user is locked out!");
            return RedirectToPage("./LockedOut", new
            {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
        }

        if (result.IsNotAllowed)
        {
            Logger.LogWarning($"External login callback error: User is Not Allowed!");

            // var user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (user.IsActive)
            {
                await StoreConfirmUserAsync(user);
                return RedirectToPage("./ConfirmUser", new
                {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash
                });
            }

            return RedirectToPage("./Login");
        }

        if (result.Succeeded)
        {
            // var user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (IsLinkLogin)
            {
                using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(user)))
                {
                    await IdentityLinkUserAppService.LinkAsync(new LinkUserInput
                    {
                        UserId = LinkUserId.Value,
                        TenantId = LinkTenantId,
                        Token = LinkToken
                    });

                    await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                    {
                        Identity = IdentitySecurityLogIdentityConsts.Identity,
                        Action = IdentityProSecurityLogActionConsts.LinkUser,
                        UserName = user.UserName,
                        ExtraProperties =
                        {
                            { IdentityProSecurityLogActionConsts.LinkTargetTenantId, LinkTenantId },
                            { IdentityProSecurityLogActionConsts.LinkTargetUserId, LinkUserId }
                        }
                    });

                    using (CurrentTenant.Change(LinkTenantId))
                    {
                        var targetUser = await UserManager.GetByIdAsync(LinkUserId.Value);
                        using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(targetUser)))
                        {
                            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                            {
                                Identity = IdentitySecurityLogIdentityConsts.Identity,
                                Action = IdentityProSecurityLogActionConsts.LinkUser,
                                UserName = targetUser.UserName,
                                ExtraProperties =
                                {
                                    { IdentityProSecurityLogActionConsts.LinkTargetTenantId, targetUser.TenantId },
                                    { IdentityProSecurityLogActionConsts.LinkTargetUserId, targetUser.Id }
                                }
                            });
                        }
                    }
                }
            }

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                Action = result.ToIdentitySecurityLogAction(),
                UserName = user.UserName
            });

            return await RedirectSafelyAsync(returnUrl, returnUrlHash);
        }

        // TODO: Handle other cases for result!
        var email = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
        if (email.IsNullOrWhiteSpace())
        {
            return RedirectToPage("./Register", new
            {
                IsExternalLogin = true,
                ExternalLoginAuthSchema = loginInfo.LoginProvider,
                ReturnUrl = returnUrl
            });
        }

        var externalUser = await UserManager.FindByEmailAsync(email);
        if (externalUser == null)
        {
            externalUser = await CreateExternalUserAsync(loginInfo);
        }
        else
        {
            if (await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey) == null)
            {
                (await UserManager.AddLoginAsync(externalUser, loginInfo)).CheckIdentityErrors();
            }
        }

        if (await HasRequiredIdentitySettingsAsync())
        {
            Logger.LogWarning($"New external user is created but confirmation is required!");

            await StoreConfirmUserAsync(externalUser);
            return RedirectToPage("./ConfirmUser", new
            {
                returnUrl = ReturnUrl,
                returnUrlHash = ReturnUrlHash
            });
        }

        await ExternalLoginSignInAsync(externalUser, loginInfo.LoginProvider, loginInfo.ProviderKey, false, true);

        if (IsLinkLogin)
        {
            using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(externalUser)))
            {
                await IdentityLinkUserAppService.LinkAsync(new LinkUserInput
                {
                    UserId = LinkUserId.Value,
                    TenantId = LinkTenantId,
                    Token = LinkToken
                });

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                {
                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                    Action = IdentityProSecurityLogActionConsts.LinkUser,
                    UserName = externalUser.UserName,
                    ExtraProperties =
                    {
                        { IdentityProSecurityLogActionConsts.LinkTargetTenantId, LinkTenantId },
                        { IdentityProSecurityLogActionConsts.LinkTargetUserId, LinkUserId }
                    }
                });

                using (CurrentTenant.Change(LinkTenantId))
                {
                    var targetUser = await UserManager.GetByIdAsync(LinkUserId.Value);
                    using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(targetUser)))
                    {
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = IdentityProSecurityLogActionConsts.LinkUser,
                            UserName = targetUser.UserName,
                            ExtraProperties =
                            {
                                { IdentityProSecurityLogActionConsts.LinkTargetTenantId, targetUser.TenantId },
                                { IdentityProSecurityLogActionConsts.LinkTargetUserId, targetUser.Id }
                            }
                        });
                    }
                }
            }
        }

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = externalUser.Name
        });

        return await RedirectSafelyAsync(returnUrl, returnUrlHash);
    }

    protected virtual async Task<SignInResult> ExternalLoginSignInAsync(IdentityUser user, string loginProvider, string providerKey, bool isPersistent, bool isSkipCheck)
    {
        if (!isSkipCheck)
        {
            if (user == null)
            {
                return SignInResult.Failed;
            }

            if (!await SignInManager.CanSignInAsync(user))
            {
                return SignInResult.NotAllowed;
            }

            if (UserManager.SupportsUserLockout && await UserManager.IsLockedOutAsync(user))
            {
                Logger.LogWarning(new EventId(3, "UserLockedOut"), "User is currently locked out.");
                return SignInResult.LockedOut;
            }
        }

        // Cleanup external cookie
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        var additionalClaims = new List<Claim>
        {
            new Claim(ClaimTypes.AuthenticationMethod, loginProvider),
            new Claim(CustomClaimTypes.ProviderKey, providerKey)
        };

        await SignInManager.SignInWithClaimsAsync(user, new AuthenticationProperties { IsPersistent = isPersistent }, additionalClaims);
        return SignInResult.Success;
    }

    protected virtual async Task<IdentityUser> CreateExternalUserAsync(ExternalLoginInfo info)
    {
        await IdentityOptions.SetAsync();

        var emailAddress = info.Principal.FindFirstValue(ClaimTypes.Email);

        var user = new IdentityUser(GuidGenerator.Create(), emailAddress, emailAddress, CurrentTenant.Id);

        (await UserManager.CreateAsync(user)).CheckIdentityErrors();
        (await UserManager.SetEmailAsync(user, emailAddress)).CheckIdentityErrors();
        (await UserManager.AddLoginAsync(user, info)).CheckIdentityErrors();
        (await UserManager.AddDefaultRolesAsync(user)).CheckIdentityErrors();

        user.Name = info.Principal.FindFirstValue(AbpClaimTypes.Name);
        user.Surname = info.Principal.FindFirstValue(AbpClaimTypes.SurName);

        var phoneNumber = info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumber);
        if (!phoneNumber.IsNullOrWhiteSpace())
        {
            var phoneNumberConfirmed = string.Equals(info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.OrdinalIgnoreCase);

            user.SetPhoneNumber(phoneNumber, phoneNumberConfirmed);
        }

        await UserManager.UpdateAsync(user);

        return user;
    }

    protected virtual async Task<bool> UseCaptchaOnLoginAsync()
    {
        return await SettingProvider.IsTrueAsync(AccountSettingNames.Captcha.UseCaptchaOnLogin);
    }

    protected virtual async Task CaptchaVerificationAsync()
    {
        UseCaptcha = await UseCaptchaOnLoginAsync();
        if (UseCaptcha)
        {
            var captchaVersion = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.Version);

            await CaptchaOptions.SetAsync(captchaVersion);

            var captchaValidator = await CaptchaValidatorFactory.CreateAsync();
            await captchaValidator.ValidateAsync(HttpContext.Request.Form[CaptchaValidatorBase.CaptchaResponseKey]);
        }
    }

    protected virtual async Task<bool> HasRequiredIdentitySettingsAsync()
    {
        var requireConfirmedEmail = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail);
        var requireConfirmedPhoneNumber = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber);
        return requireConfirmedEmail || requireConfirmedPhoneNumber;
    }

    protected virtual Task<IActionResult> OnCaptchaScoreBelowThreshold()
    {
        return null;
    }

    protected new virtual async Task<IActionResult> CheckLocalLoginAsync()
    {
        ExternalProviders = await GetExternalProvidersAsync();
        EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

        if (!EnableLocalLogin && IsExternalLoginOnly && ExternalLoginScheme != null)
        {
            return await OnPostExternalLoginAsync(ExternalLoginScheme);
        }

        if (!EnableLocalLogin)
        {
            Alerts.Warning(L["LocalLoginIsNotEnabled"]);
            return Page();
        }

        return null;
    }

    public class LoginInputModel
    {
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ExternalProviderModel
    {
        public string DisplayName { get; set; }

        public string AuthenticationScheme { get; set; }

        public string Icon { get; set; }
    }
}
