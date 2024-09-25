// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;

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

using static Volo.Abp.Identity.Settings.IdentitySettingNames;

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

    public IEnumerable<ExternalProviderModel> VisibleExternalProviders
    {
        get
        {
            return ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));
        }
    }

    public bool IsExternalLoginOnly
    {
        get
        {
            if (EnableLocalLogin)
            {
                return false;
            }

            return ExternalProviders != null && ExternalProviders.Count() == 1;
        }
    }

    public string ExternalLoginScheme
    {
        get
        {
            if (!IsExternalLoginOnly)
            {
                return null;
            }

            if (ExternalProviders == null)
            {
                return null;
            }

            return ExternalProviders.SingleOrDefault()?.AuthenticationScheme;
        }
    }

    protected IAuthenticationSchemeProvider SchemeProvider { get; }

    protected AbpAccountOptions AccountOptions { get; }

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

    protected IAbpCaptchaValidatorFactory CaptchaValidatorFactory { get; }

    protected IAccountExternalProviderAppService AccountExternalProviderAppService { get; }

    public LoginModel(
        IAuthenticationSchemeProvider schemeProvider,
        IOptions<AbpAccountOptions> accountOptions,
        IAbpCaptchaValidatorFactory captchaValidatorFactory,
        IAccountExternalProviderAppService accountExternalProviderAppService,
        ICurrentPrincipalAccessor currentPrincipalAccessor)
    {
        SchemeProvider = schemeProvider;
        AccountOptions = accountOptions.Value;
        CaptchaValidatorFactory = captchaValidatorFactory;
        AccountExternalProviderAppService = accountExternalProviderAppService;
        CurrentPrincipalAccessor = currentPrincipalAccessor;
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
        if (IsLinkLogin && CurrentUser.IsAuthenticated)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.Logout
            });

            await SignInManager.SignOutAsync();

            return Redirect(HttpContext.Request.GetDisplayUrl());
        }

        return Page();
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync(string action)
    {
        try
        {
            await CaptchaVerificationAsync();
        }
        catch (UserFriendlyException ex)
        {
            if (ex is ScoreBelowThresholdException)
            {
                var onScoreBelowThresholdResult = await OnCaptchaScoreBelowThresholdAsync();
                if (onScoreBelowThresholdResult != null)
                {
                    return onScoreBelowThresholdResult;
                }
            }

            Alerts.Danger(GetLocalizeExceptionMessage(ex));
            return Page();
        }

        ValidateModel();
        await IdentityOptions.SetAsync();

        IActionResult localLogin = await CheckLocalLoginAsync();
        if (localLogin != null)
        {
            return localLogin;
        }

        IsSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);

        await ReplaceEmailToUsernameOfInputIfNeedsAsync();

        IsLinkLogin = await VerifyLinkTokenAsync();

        SignInResult result = await SignInManager.PasswordSignInAsync(LoginInput.UserNameOrEmailAddress, LoginInput.Password, LoginInput.RememberMe, true);

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = result.ToIdentitySecurityLogAction(),
            UserName = LoginInput.UserNameOrEmailAddress
        });

        if (result.RequiresTwoFactor)
        {
            return RedirectToPage("./SendSecurityCode", new
            {
                ReturnUrl,
                ReturnUrlHash,
                LoginInput.RememberMe,
                LinkUserId,
                LinkTenantId,
                LinkToken
            });
        }

        if (result.IsLockedOut)
        {
            IdentityUser user = await GetIdentityUserAsync(LoginInput.UserNameOrEmailAddress);
            await StoreLockedUserAsync(user);
            return RedirectToPage("./LockedOut", new
            {
                ReturnUrl,
                ReturnUrlHash
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
                        notAllowedUser = await UserManager.GetByIdAsync(LinkUserId.Value);
                        using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(notAllowedUser)))
                        {
                            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                            {
                                Identity = IdentitySecurityLogIdentityConsts.Identity,
                                Action = IdentityProSecurityLogActionConsts.LinkUser,
                                UserName = notAllowedUser.UserName,
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
                        ReturnUrl,
                        ReturnUrlHash,
                        LinkUserId,
                        LinkTenantId
                    });
                }
            }
            else
            {
                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
                return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
            }
        }
        else
        {
            notAllowedUser = await GetIdentityUserAsync(LoginInput.UserNameOrEmailAddress);
            await StoreLockedUserAsync(notAllowedUser);
            if (!await UserManager.CheckPasswordAsync(notAllowedUser, LoginInput.Password))
            {
                Alerts.Danger(L["LoginIsNotAllowed"]);
                return Page();
            }

            if (notAllowedUser.ShouldChangePasswordOnNextLogin || await UserManager.ShouldPeriodicallyChangePasswordAsync(notAllowedUser))
            {
                await StoreChangePasswordUserAsync(notAllowedUser);
                return RedirectToPage("./ChangePassword", new
                {
                    ReturnUrl,
                    ReturnUrlHash,
                    LoginInput.RememberMe
                });
            }
            else
            {
                if (notAllowedUser.IsActive || await UserManager.CheckPasswordAsync(notAllowedUser, LoginInput.Password))
                {
                    await StoreConfirmUserAsync(notAllowedUser);
                    return RedirectToPage("./ConfirmUser", new
                    {
                        ReturnUrl,
                        ReturnUrlHash
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

    protected virtual async Task<IdentityUser> GetIdentityUserAsync(string userNameOrEmailAddress)
    {
        // TODO: Find a way of getting user's id from the logged in user and do not query it again like that!
        return await UserManager.FindByNameAsync(LoginInput.UserNameOrEmailAddress) ??
            await UserManager.FindByEmailAsync(LoginInput.UserNameOrEmailAddress);
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
        if (AbpStringExtensions.IsNullOrWhiteSpace(LinkToken) || !LinkUserId.HasValue)
        {
            return false;
        }

        return await IdentityLinkUserAppService.VerifyLinkTokenAsync(new VerifyLinkTokenInput()
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
        var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new
        {
            ReturnUrl,
            ReturnUrlHash,
            LinkTenantId,
            LinkUserId,
            LinkToken
        });
        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    [UnitOfWork]
    public virtual async Task<IActionResult> OnGetExternalLoginCallbackAsync(string remoteError = null)
    {
        if (remoteError != null)
        {
            Logger.LogWarning("External login callback error: {RemoteError}", remoteError);
            return RedirectToPage("./Login");
        }

        await IdentityOptions.SetAsync();
        ExternalLoginInfo loginInfo = await SignInManager.GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            Logger.LogWarning("External login info is not available");
            return RedirectToPage("./Login");
        }

        IsLinkLogin = await VerifyLinkTokenAsync();

        SignInResult result = await SignInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true, true);
        if (!result.Succeeded)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                Action = "Login" + result.ToString()
            });
        }

        if (!result.IsLockedOut)
        {
            IdentityUser user;
            if (result.IsNotAllowed)
            {
                Logger.LogWarning("External login callback error: User is Not Allowed!");

                user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
                if (user == null)
                {
                    Logger.LogWarning($"External login callback error: User is Not Found!");
                    return RedirectToPage("./Login");
                }

                if (user.ShouldChangePasswordOnNextLogin || await UserManager.ShouldPeriodicallyChangePasswordAsync(user))
                {
                    await StoreChangePasswordUserAsync(user);
                    return RedirectToPage("./ChangePassword", new
                    {
                        ReturnUrl,
                        ReturnUrlHash
                    });
                }
                else
                {
                    if (user.IsActive)
                    {
                        await StoreConfirmUserAsync(user);
                        return RedirectToPage("./ConfirmUser", new
                        {
                            ReturnUrl,
                            ReturnUrlHash
                        });
                    }
                    else
                    {
                        return RedirectToPage("./Login");
                    }
                }
            }

            if (!result.Succeeded)
            {
                string email = loginInfo.Principal.FindFirstValue(AbpClaimTypes.Email) ?? loginInfo.Principal.FindFirstValue(JwtClaimTypes.Email);
                if (email.IsNullOrWhiteSpace())
                {
                    return RedirectToPage("./Register", new
                    {
                        isExternalLogin = true,
                        externalLoginAuthSchema = loginInfo.LoginProvider,
                        returnUrl = ReturnUrl,
                        returnUrlHash = ReturnUrlHash,
                        linkTenantId = LinkTenantId,
                        linkUserId = LinkUserId,
                        linkToken = LinkToken
                    });
                }

                IdentityUser externalUser = await UserManager.FindByEmailAsync(email);
                if (externalUser == null)
                {
                    return RedirectToPage("./Login", new
                    {
                        isExternalLogin = true,
                        externalLoginAuthSchema = loginInfo.LoginProvider,
                        returnUrl = ReturnUrl,
                        returnUrlHash = ReturnUrlHash,
                        linkTenantId = LinkTenantId,
                        linkUserId = LinkUserId,
                        linkToken = LinkToken
                    });
                }

                if (await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey) == null)
                {
                    IdentityResult identityResult = await UserManager.AddLoginAsync(externalUser, loginInfo);
                    CheckIdentityErrors(identityResult);
                }

                if (await HasRequiredIdentitySettingsAsync())
                {
                    Logger.LogWarning("New external user is created but confirmation is required!");

                    await StoreConfirmUserAsync(externalUser);
                    return RedirectToPage("./ConfirmUser", new
                    {
                        ReturnUrl,
                        ReturnUrlHash
                    });
                }

                await SignInManager.SignInAsync(externalUser, false, loginInfo.LoginProvider);
                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(externalUser.Id, externalUser.TenantId);
                if (IsLinkLogin)
                {
                    using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(externalUser)))
                    {
                        LinkUserInput input = new LinkUserInput
                        {
                            UserId = LinkUserId.Value,
                            TenantId = LinkTenantId,
                            Token = LinkToken
                        };
                        await IdentityLinkUserAppService.LinkAsync(input);
                        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                        {
                            Identity = IdentitySecurityLogIdentityConsts.Identity,
                            Action = IdentityProSecurityLogActionConsts.LinkUser,
                            UserName = externalUser.UserName,
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
                            user = await UserManager.GetByIdAsync(LinkUserId.Value);
                            using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(user)))
                            {
                                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                                {
                                    Identity = IdentitySecurityLogIdentityConsts.Identity,
                                    Action = IdentityProSecurityLogActionConsts.LinkUser,
                                    UserName = user.UserName,
                                    ExtraProperties =
                                    {
                                        {
                                            IdentityProSecurityLogActionConsts.LinkTargetTenantId,
                                            user.TenantId
                                        },
                                        {
                                            IdentityProSecurityLogActionConsts.LinkTargetUserId,
                                            user.Id
                                        }
                                    }
                                });
                            }
                        }
                    }
                }

                await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
                {
                    Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                    Action = result.ToIdentitySecurityLogAction(),
                    UserName = externalUser.Name
                });
                return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
            }

            user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            if (IsLinkLogin)
            {
                using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(user)))
                {
                    LinkUserInput input = new LinkUserInput
                    {
                        UserId = LinkUserId.Value,
                        TenantId = LinkTenantId,
                        Token = LinkToken
                    };
                    await IdentityLinkUserAppService.LinkAsync(input);
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

            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext()
            {
                Identity = IdentitySecurityLogIdentityConsts.IdentityExternal,
                Action = result.ToIdentitySecurityLogAction(),
                UserName = user.UserName
            });
            await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
            return await RedirectSafelyAsync(ReturnUrl, ReturnUrlHash);
        }
        else
        {
            IdentityUser user = await UserManager.FindByLoginAsync(loginInfo.LoginProvider, loginInfo.ProviderKey);
            await StoreLockedUserAsync(user);
            Logger.LogWarning("Cannot proceed because user is locked out!");
            return RedirectToPage("./LockedOut", new
            {
                ReturnUrl,
                ReturnUrlHash
            });
        }
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
            new Claim(ExtraClaimTypes.ProviderKey, providerKey)
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
        return requireConfirmedEmail | requireConfirmedPhoneNumber;
    }

    protected virtual Task<IActionResult> OnCaptchaScoreBelowThresholdAsync()
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

        if (EnableLocalLogin)
        {
            return null;
        }

        Alerts.Warning(L["LocalLoginIsNotEnabled"]);
        return Page();
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
