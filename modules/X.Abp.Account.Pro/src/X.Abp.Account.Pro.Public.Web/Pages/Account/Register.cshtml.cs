// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;
using IdentityModel.Client;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Content;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Reflection;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Validation;

using X.Abp.Account.Dtos;
using X.Abp.Account.ExternalProviders;
using X.Abp.Account.Public.Web.Security.Captcha;
using X.Abp.Account.Settings;
using X.Abp.Identity;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account
{
    public class RegisterModel : AccountPageModel
    {
        protected IApplicationInfoAccessor ApplicationInfoAccessor { get; }

        protected IAuthenticationSchemeProvider SchemeProvider { get; }

        protected AbpAccountOptions AccountOptions { get; }

        protected IAccountExternalProviderAppService AccountExternalProviderAppService { get; }

        protected ICurrentPrincipalAccessor CurrentPrincipalAccessor { get; }

        protected IHttpClientFactory HttpClientFactory { get; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrlHash { get; set; }

        [BindProperty]
        public PostInput Input { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IsExternalLogin { get; set; }

        [BindProperty(SupportsGet = true)]
        public string ExternalLoginAuthSchema { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public Guid? LinkUserId { get; set; }

        [BindProperty(SupportsGet = true)]
        [HiddenInput]
        public Guid? LinkTenantId { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string LinkToken { get; set; }

        public bool UseCaptcha { get; set; }

        public IEnumerable<ExternalProviderModel> ExternalProviders { get; set; }

        public IEnumerable<ExternalProviderModel> VisibleExternalProviders => ExternalProviders.Where(x => !string.IsNullOrWhiteSpace(x.DisplayName));

        public bool EnableLocalRegister { get; set; }

        public bool EnableLocalLogin { get; set; }

        public bool IsExternalLoginOnly
        {
            get
            {
                if (EnableLocalRegister)
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

        public RegisterModel(
          IApplicationInfoAccessor applicationInfoAccessor,
          IAuthenticationSchemeProvider schemeProvider,
          IOptions<AbpAccountOptions> accountOptions,
          IAccountExternalProviderAppService accountExternalProviderAppService,
          ICurrentPrincipalAccessor currentPrincipalAccessor,
          IHttpClientFactory httpClientFactory)
        {
            ApplicationInfoAccessor = applicationInfoAccessor;
            SchemeProvider = schemeProvider;
            AccountOptions = accountOptions.Value;
            AccountExternalProviderAppService = accountExternalProviderAppService;
            CurrentPrincipalAccessor = currentPrincipalAccessor;
            HttpClientFactory = httpClientFactory;
        }

        public virtual async Task<IActionResult> OnGetAsync()
        {
            ExternalProviders = await GetExternalProviders();
            if (!await CheckSelfRegistrationAsync())
            {
                if (IsExternalLoginOnly)
                {
                    return await OnPostExternalLogin(ExternalLoginScheme);
                }

                Alerts.Warning(L["Volo.Account:SelfRegistrationDisabled"]);
                return Page();
            }

            await TrySetEmailAsync();
            await SetUseCaptchaAsync();
            return Page();
        }

        [UnitOfWork]
        public virtual async Task<IActionResult> OnPostAsync()
        {
            try
            {
                ExternalProviders = await GetExternalProviders();

                if (!await CheckSelfRegistrationAsync())
                {
                    throw new UserFriendlyException(L["Volo.Account:SelfRegistrationDisabled"]);
                }

                await SetUseCaptchaAsync();
                IdentityUser user;
                if (IsExternalLogin)
                {
                    ExternalLoginInfo externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
                    if (externalLoginInfo == null)
                    {
                        Logger.LogWarning("External login info is not available");
                        return RedirectToPage("./Login");
                    }

                    if (Input.UserName.IsNullOrWhiteSpace())
                    {
                        Input.UserName = await UserManager.GetUserNameFromEmailAsync(Input.EmailAddress);
                    }

                    user = await RegisterExternalUserAsync(externalLoginInfo, Input.UserName, Input.EmailAddress);
                    await SignInManager.SignInAsync(user, true, ExternalLoginAuthSchema);
                    await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);
                }
                else
                {
                    user = await RegisterLocalUserAsync();
                }

                bool isConfirmed;
                if (!(isConfirmed = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail) && !user.EmailConfirmed))
                {
                    isConfirmed = await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail) && !user.PhoneNumberConfirmed;
                }

                if (isConfirmed)
                {
                    await StoreConfirmUserAsync(user);
                    return RedirectToPage("./ConfirmUser", new
                    {
                        returnUrl = ReturnUrl,
                        returnUrlHash = ReturnUrlHash
                    });
                }

                if (await VerifyLinkTokenAsync())
                {
                    using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(user)))
                    {
                        await IdentityLinkUserAppService.LinkAsync(new LinkUserInput()
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

                await SignInManager.SignInAsync(user, true, null);
                await IdentityDynamicClaimsPrincipalContributorCache.ClearAsync(user.Id, user.TenantId);

                return Redirect(ReturnUrl ?? "/");
            }
            catch (BusinessException ex)
            {
                Alerts.Danger(GetLocalizeExceptionMessage(ex));
                return Page();
            }
        }

        protected virtual async Task<IdentityUser> RegisterLocalUserAsync()
        {
            ValidateModel();
            string captchaResponse = string.Empty;
            if (UseCaptcha)
            {
                captchaResponse = HttpContext.Request.Form[CaptchaValidatorBase.CaptchaResponseKey];
            }

            IdentityUserDto identityUserDto = await AccountAppService.RegisterAsync(new RegisterDto()
            {
                AppName = ApplicationInfoAccessor.ApplicationName,
                EmailAddress = Input.EmailAddress,
                Password = Input.Password,
                UserName = Input.UserName,
                ReturnUrl = ReturnUrl,
                ReturnUrlHash = ReturnUrlHash,
                CaptchaResponse = captchaResponse
            });
            return await UserManager.GetByIdAsync(identityUserDto.Id);
        }

        protected virtual async Task<IdentityUser> RegisterExternalUserAsync(
          ExternalLoginInfo externalLoginInfo,
          string userName,
          string emailAddress)
        {
            await IdentityOptions.SetAsync();
            IdentityUser user = new IdentityUser(GuidGenerator.Create(), userName, emailAddress, CurrentTenant.Id);
            (await UserManager.CreateAsync(user)).CheckIdentityErrors();
            (await UserManager.AddDefaultRolesAsync(user)).CheckIdentityErrors();
            if (!user.EmailConfirmed)
            {
                await AccountAppService.SendEmailConfirmationTokenAsync(new SendEmailConfirmationTokenDto()
                {
                    AppName = ApplicationInfoAccessor.ApplicationName,
                    UserId = user.Id,
                    ReturnUrl = ReturnUrl,
                    ReturnUrlHash = ReturnUrlHash
                });
            }

            user.Name = externalLoginInfo.Principal.FindFirstValue(AbpClaimTypes.Name) ?? externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Name) ?? externalLoginInfo.Principal.FindFirstValue(ClaimTypes.GivenName);
            user.Surname = externalLoginInfo.Principal.FindFirstValue(AbpClaimTypes.SurName) ?? externalLoginInfo.Principal.FindFirstValue(JwtClaimTypes.FamilyName);
            string phoneNumber = externalLoginInfo.Principal.FindFirstValue(AbpClaimTypes.PhoneNumber);
            if (!phoneNumber.IsNullOrWhiteSpace())
            {
                bool confirmed = string.Equals(externalLoginInfo.Principal.FindFirstValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.OrdinalIgnoreCase);
                user.SetPhoneNumber(phoneNumber, confirmed);
            }

            string picture = externalLoginInfo.Principal.FindFirstValue(AbpClaimTypes.Picture);
            if (!picture.IsNullOrWhiteSpace())
            {
                using HttpClient client = HttpClientFactory.CreateClient();
                if (externalLoginInfo.AuthenticationTokens != null && externalLoginInfo.AuthenticationTokens.Any())
                {
                    string token = externalLoginInfo.AuthenticationTokens.FirstOrDefault(x => x.Name == "access_token")?.Value;
                    client.SetBearerToken(token);
                }

                try
                {
                    Stream imageSteam = await client.GetStreamAsync(picture);
                    using (CurrentPrincipalAccessor.Change(await SignInManager.CreateUserPrincipalAsync(user)))
                    {
                        await AccountAppService.SetProfilePictureAsync(new ProfilePictureInput()
                        {
                            ImageContent = new RemoteStreamContent(imageSteam),
                            Type = ProfilePictureType.Image
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, $"Failed to save profile picture for user: {user.Id}");
                }
            }

            var userLoginAlreadyExists = user.Logins.Any(x =>
    x.TenantId == user.TenantId &&
    x.LoginProvider == externalLoginInfo.LoginProvider &&
    x.ProviderKey == externalLoginInfo.ProviderKey);

            if (!userLoginAlreadyExists)
            {
                user.AddLogin(new UserLoginInfo(
                        externalLoginInfo.LoginProvider,
                        externalLoginInfo.ProviderKey,
                        externalLoginInfo.ProviderDisplayName));

                (await UserManager.UpdateAsync(user)).CheckIdentityErrors();
            }

            return user;
        }

        protected virtual async Task<bool> CheckSelfRegistrationAsync()
        {
            // throw new UserFriendlyException(L["SelfRegistrationDisabledMessage"]);
            EnableLocalLogin = await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin);

            bool isSelfRegistrationEnabled = await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled);
            EnableLocalRegister = isSelfRegistrationEnabled && EnableLocalLogin;
            return IsExternalLogin || EnableLocalRegister;
        }

        protected virtual async Task SetUseCaptchaAsync()
        {
            UseCaptcha = !IsExternalLogin && await SettingProvider.IsTrueAsync(AccountSettingNames.Captcha.UseCaptchaOnRegistration);
            if (UseCaptcha)
            {
                var captchaVersion = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.Version);
                await CaptchaOptions.SetAsync(captchaVersion);
            }
        }

        protected virtual new async Task StoreConfirmUserAsync(IdentityUser user)
        {
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(ConfirmUserModel.ConfirmUserScheme);
            claimsIdentity.AddClaim(new Claim(AbpClaimTypes.UserId, user.Id.ToString()));
            if (user.TenantId.HasValue)
            {
                claimsIdentity.AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
            }

            await HttpContext.SignInAsync(ConfirmUserModel.ConfirmUserScheme, new ClaimsPrincipal(claimsIdentity));
        }

        protected virtual async Task<List<ExternalProviderModel>> GetExternalProviders()
        {
            IEnumerable<AuthenticationScheme> schemes = await SchemeProvider.GetAllSchemesAsync();
            ExternalProviderDto externalProviders = await AccountExternalProviderAppService.GetAllAsync();
            List<ExternalProviderModel> externalProviderModelList = new List<ExternalProviderModel>();
            foreach (AuthenticationScheme scheme in schemes)
            {
                if (IsRemoteAuthenticationHandler(scheme, externalProviders) || scheme.Name.Equals(AccountOptions.WindowsAuthenticationSchemeName, StringComparison.OrdinalIgnoreCase))
                {
                    externalProviderModelList.Add(new ExternalProviderModel()
                    {
                        DisplayName = scheme.DisplayName,
                        AuthenticationScheme = scheme.Name,
                        Icon = AccountOptions.ExternalProviderIconMap.GetOrDefault(scheme.Name)
                    });
                }
            }

            return externalProviderModelList;
        }

        protected virtual bool IsRemoteAuthenticationHandler(
          AuthenticationScheme scheme,
          ExternalProviderDto externalProviders)
        {
            if (!ReflectionHelper.IsAssignableToGenericType(scheme.HandlerType, typeof(RemoteAuthenticationHandler<>)))
            {
                return false;
            }

            ExternalProviderItemDto externalProviderItemDto = externalProviders.Providers.FirstOrDefault(x => x.Name == scheme.Name);
            return externalProviderItemDto == null || externalProviderItemDto.Enabled;
        }

        [UnitOfWork]
        public virtual async Task<IActionResult> OnPostExternalLogin(string provider)
        {
            var redirectUrl = Url.Page("./Login", pageHandler: "ExternalLoginCallback", values: new { ReturnUrl, ReturnUrlHash });
            var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            properties.Items["scheme"] = provider;

            return await Task.FromResult(Challenge(properties, provider));
        }

        protected virtual async Task TrySetEmailAsync()
        {
            if (!IsExternalLogin)
            {
                return;
            }

            ExternalLoginInfo externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null || !externalLoginInfo.Principal.Identities.Any())
            {
                return;
            }

            ClaimsIdentity claimsIdentity = externalLoginInfo.Principal.Identities.First();
            Claim emailClaim = claimsIdentity.FindFirst(AbpClaimTypes.Email) ?? claimsIdentity.FindFirst(JwtClaimTypes.Email);
            if (emailClaim == null)
            {
                return;
            }

            string str = await UserManager.GetUserNameFromEmailAsync(emailClaim.Value);
            Input = new PostInput()
            {
                UserName = str,
                EmailAddress = emailClaim.Value
            };
        }

        protected virtual async Task<bool> VerifyLinkTokenAsync()
        {
            if (LinkToken.IsNullOrWhiteSpace() || !LinkUserId.HasValue)
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

        public class PostInput
        {
            [DynamicStringLength(typeof(IdentityUserConsts), "MaxUserNameLength", null)]
            [Required]
            public string UserName { get; set; }

            [DynamicStringLength(typeof(IdentityUserConsts), "MaxEmailLength", null)]
            [EmailAddress]
            [Required]
            public string EmailAddress { get; set; }

            [DynamicStringLength(typeof(IdentityUserConsts), "MaxPasswordLength", null)]
            [DataType(DataType.Password)]
            [DisableAuditing]
            [Required]
            public string Password { get; set; }
        }

        public class ExternalProviderModel
        {
            public string DisplayName { get; set; }

            public string AuthenticationScheme { get; set; }

            public string Icon { get; set; }
        }
    }
}
