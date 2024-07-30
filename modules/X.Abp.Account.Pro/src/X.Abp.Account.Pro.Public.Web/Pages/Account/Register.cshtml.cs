// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Security.Claims;
using Volo.Abp.Settings;
using Volo.Abp.Uow;
using Volo.Abp.Validation;

using X.Abp.Account.Dtos;
using X.Abp.Account.Public.Web.Security.Captcha;
using X.Abp.Account.Settings;
using X.Captcha;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account.Public.Web.Pages.Account;

public class RegisterModel : AccountPageModel
{
    [BindProperty(SupportsGet = true)]
    public string ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ReturnUrlHash { get; set; }

    [BindProperty]
    public PostInput Input { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IsExternalLogin { get; set; }

    public bool LocalLoginDisabled { get; set; }

    public bool UseCaptcha { get; set; }

    protected IAccountAppService AccountAppService { get; }

    protected SignInManager<IdentityUser> SignInManager { get; }

    protected IdentityUserManager UserManager { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    protected IOptionsSnapshot<CaptchaOptions> CaptchaOptions { get; }

    public RegisterModel(
        IAccountAppService accountAppService,
        SignInManager<IdentityUser> signInManager,
        IdentityUserManager userManager,
        IOptions<IdentityOptions> identityOptions,
        IOptionsSnapshot<CaptchaOptions> captchaOptions)
    {
        AccountAppService = accountAppService;
        SignInManager = signInManager;
        UserManager = userManager;
        IdentityOptions = identityOptions;
        CaptchaOptions = captchaOptions;
    }

    public virtual async Task<IActionResult> OnGetAsync()
    {
        var localLoginResult = await CheckLocalLoginAsync();
        if (localLoginResult != null)
        {
            LocalLoginDisabled = true;
            return localLoginResult;
        }

        await CheckSelfRegistrationAsync();
        await TrySetEmailAsync();
        await SetUseCaptchaAsync();

        return Page();
    }

    // TODO: Will be removed when we implement action filter
    [UnitOfWork]
    public virtual async Task<IActionResult> OnPostAsync()
    {
        try
        {
            await CheckSelfRegistrationAsync();
            await SetUseCaptchaAsync();

            IdentityUser user;
            if (IsExternalLogin)
            {
                var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
                if (externalLoginInfo == null)
                {
                    Logger.LogWarning("External login info is not available");
                    return RedirectToPage("./Login");
                }

                user = await RegisterExternalUserAsync(externalLoginInfo, Input.EmailAddress);
            }
            else
            {
                var localLoginResult = await CheckLocalLoginAsync();
                if (localLoginResult != null)
                {
                    LocalLoginDisabled = true;
                    return localLoginResult;
                }

                user = await RegisterLocalUserAsync();
            }

            if ((await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail) && !user.EmailConfirmed) ||
                (await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedPhoneNumber) && !user.PhoneNumberConfirmed))
            {
                await StoreConfirmUserAsync(user);

                return RedirectToPage("./ConfirmUser", new
                {
                    returnUrl = ReturnUrl,
                    returnUrlHash = ReturnUrlHash
                });
            }

            await SignInManager.SignInAsync(user, isPersistent: true);

            // TODO: How to ensure safety? IdentityServer requires it however it should be checked somehow!
            return Redirect(ReturnUrl ?? "/");
        }
        catch (BusinessException e)
        {
            Alerts.Danger(GetLocalizeExceptionMessage(e));
            return Page();
        }
    }

    protected virtual async Task<IdentityUser> RegisterLocalUserAsync()
    {
        ValidateModel();

        var captchaResponse = string.Empty;
        if (UseCaptcha)
        {
            captchaResponse = HttpContext.Request.Form[CaptchaValidatorBase.CaptchaResponseKey];
        }

        var userDto = await AccountAppService.RegisterAsync(
            new RegisterDto
            {
                AppName = "MVC",
                EmailAddress = Input.EmailAddress,
                Password = Input.Password,
                UserName = Input.UserName,
                ReturnUrl = ReturnUrl,
                ReturnUrlHash = ReturnUrlHash,
                CaptchaResponse = captchaResponse
            });

        return await UserManager.GetByIdAsync(userDto.Id);
    }

    protected virtual async Task<IdentityUser> RegisterExternalUserAsync(ExternalLoginInfo externalLoginInfo, string emailAddress)
    {
        await IdentityOptions.SetAsync();

        var user = new IdentityUser(GuidGenerator.Create(), emailAddress, emailAddress, CurrentTenant.Id);

        (await UserManager.CreateAsync(user)).CheckIdentityErrors();
        (await UserManager.AddDefaultRolesAsync(user)).CheckIdentityErrors();

        if (!user.EmailConfirmed)
        {
            await AccountAppService.SendEmailConfirmationTokenAsync(
                new SendEmailConfirmationTokenDto
                {
                    AppName = "MVC",
                    UserId = user.Id,
                    ReturnUrl = ReturnUrl,
                    ReturnUrlHash = ReturnUrlHash
                });
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

    protected virtual async Task CheckSelfRegistrationAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled) ||
            !await SettingProvider.IsTrueAsync(AccountSettingNames.EnableLocalLogin))
        {
            throw new UserFriendlyException(L["SelfRegistrationDisabledMessage"]);
        }
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

    protected override async Task StoreConfirmUserAsync(IdentityUser user)
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

    private async Task TrySetEmailAsync()
    {
        if (IsExternalLogin)
        {
            var externalLoginInfo = await SignInManager.GetExternalLoginInfoAsync();
            if (externalLoginInfo == null)
            {
                return;
            }

            if (!externalLoginInfo.Principal.Identities.Any())
            {
                return;
            }

            var identity = externalLoginInfo.Principal.Identities.First();
            var emailClaim = identity.FindFirst(ClaimTypes.Email);

            if (emailClaim == null)
            {
                return;
            }

            Input = new PostInput { EmailAddress = emailClaim.Value };
        }
    }

    public class PostInput
    {
        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxUserNameLength))]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxEmailLength))]
        public string EmailAddress { get; set; }

        [Required]
        [DynamicStringLength(typeof(IdentityUserConsts), nameof(IdentityUserConsts.MaxPasswordLength))]
        [DataType(DataType.Password)]
        [DisableAuditing]
        public string Password { get; set; }
    }
}
