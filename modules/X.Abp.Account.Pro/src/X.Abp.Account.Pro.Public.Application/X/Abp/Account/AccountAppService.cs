// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.BlobStoring;
using Volo.Abp.Content;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.ObjectExtending;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Users;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Account.Dtos;
using X.Abp.Account.Emailing;
using X.Abp.Account.ExternalProviders;
using X.Abp.Account.Localization;
using X.Abp.Account.Phone;
using X.Abp.Account.Security.Captcha;
using X.Abp.Account.Settings;
using X.Abp.Identity;
using X.Captcha;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account;

public class AccountAppService : ApplicationService, IAccountAppService
{
    protected IConfiguration Configuration => LazyServiceProvider.LazyGetRequiredService<IConfiguration>();

    protected ExternalProviderSettingsHelper ExternalProviderSettingsHelper => LazyServiceProvider.LazyGetRequiredService<ExternalProviderSettingsHelper>();

    protected IdentityUserManager UserManager { get; }

    protected IAccountEmailer AccountEmailer { get; }

    protected IAccountPhoneService PhoneService { get; }

    protected IdentitySecurityLogManager IdentitySecurityLogManager { get; }

    public IAbpCaptchaValidatorFactory CaptchaValidatorFactory { get; set; }

    protected ISettingManager SettingManager { get; }

    protected IBlobContainer<AccountProfilePictureContainer> AccountProfilePictureContainer { get; }

    protected IOptionsSnapshot<CaptchaOptions> CaptchaOptions { get; }

    protected IOptions<IdentityOptions> IdentityOptions { get; }

    protected IIdentitySecurityLogRepository SecurityLogRepository { get; }

    public AccountAppService(
        IdentityUserManager userManager,
        IAccountEmailer accountEmailer,
        IAccountPhoneService phoneService,
        IdentitySecurityLogManager identitySecurityLogManager,
        IBlobContainer<AccountProfilePictureContainer> accountProfilePictureContainer,
        ISettingManager settingManager,
        IOptionsSnapshot<CaptchaOptions> captchaOptions,
        IOptions<IdentityOptions> identityOptions,
        IIdentitySecurityLogRepository securityLogRepository)
    {
        IdentitySecurityLogManager = identitySecurityLogManager;
        UserManager = userManager;
        AccountEmailer = accountEmailer;
        PhoneService = phoneService;
        AccountProfilePictureContainer = accountProfilePictureContainer;
        SettingManager = settingManager;
        CaptchaOptions = captchaOptions;
        IdentityOptions = identityOptions;
        SecurityLogRepository = securityLogRepository;

        LocalizationResource = typeof(AccountResource);
        CaptchaValidatorFactory = NullAbpCaptchaValidatorFactory.Instance;
    }

    public virtual async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        await CheckSelfRegistrationAsync();

        if (await UseCaptchaOnRegistrationAsync())
        {
            var captchaVersion = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.Version);
            await CaptchaOptions.SetAsync(captchaVersion);

            var captchaValidator = await CaptchaValidatorFactory.CreateAsync();
            await captchaValidator.ValidateAsync(input.CaptchaResponse);
        }

        await IdentityOptions.SetAsync();

        var user = new IdentityUser(GuidGenerator.Create(), input.UserName, input.EmailAddress, CurrentTenant.Id);

        input.MapExtraPropertiesTo(user);

        (await UserManager.CreateAsync(user, input.Password)).CheckIdentityErrors();
        (await UserManager.AddDefaultRolesAsync(user)).CheckIdentityErrors();

        if (!user.EmailConfirmed)
        {
            await SendEmailConfirmationTokenAsync(user, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
        }

        return ObjectMapper.Map<IdentityUser, IdentityUserDto>(user);
    }

    public virtual async Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
    {
        var user = await GetUserByEmailAsync(input.Email);
        var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
        await AccountEmailer.SendPasswordResetLinkAsync(user, resetToken, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
    }

    public virtual async Task ResetPasswordAsync(ResetPasswordDto input)
    {
        await IdentityOptions.SetAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);
        (await UserManager.ResetPasswordAsync(user, input.ResetToken, input.Password)).CheckIdentityErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePassword
        });
    }

    public async Task<IdentityUserConfirmationStateDto> GetConfirmationStateAsync(Guid id)
    {
        var user = await UserManager.GetByIdAsync(id);

        return new IdentityUserConfirmationStateDto
        {
            PhoneNumberConfirmed = user.PhoneNumberConfirmed,
            EmailConfirmed = user.EmailConfirmed
        };
    }

    public virtual async Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input)
    {
        await CheckIfPhoneNumberConfirmationEnabledAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);

        if (!input.PhoneNumber.IsNullOrWhiteSpace())
        {
            (await UserManager.SetPhoneNumberAsync(user, input.PhoneNumber)).CheckIdentityErrors();
        }

        CheckPhoneNumber(user);

        var token = await UserManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
        await PhoneService.SendConfirmationCodeAsync(user, token);
    }

    public async Task SendEmailConfirmationTokenAsync(SendEmailConfirmationTokenDto input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        await SendEmailConfirmationTokenAsync(user, input.AppName, input.ReturnUrl, input.ReturnUrlHash);
    }

    protected virtual async Task SendEmailConfirmationTokenAsync(
        IdentityUser user,
        string applicationName,
        string returnUrl,
        string returnUrlHash)
    {
        var confirmationToken = await UserManager.GenerateEmailConfirmationTokenAsync(user);
        await AccountEmailer.SendEmailConfirmationLinkAsync(user, confirmationToken, applicationName, returnUrl, returnUrlHash);
    }

    public virtual async Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input)
    {
        await CheckIfPhoneNumberConfirmationEnabledAsync();

        var user = await UserManager.GetByIdAsync(input.UserId);

        CheckPhoneNumber(user);

        (await UserManager.ChangePhoneNumberAsync(user, user.PhoneNumber, input.Token)).CheckIdentityErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangePhoneNumber
        });
    }

    public virtual async Task ConfirmEmailAsync(ConfirmEmailInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (user.EmailConfirmed)
        {
            return;
        }

        (await UserManager.ConfirmEmailAsync(user, input.Token)).CheckIdentityErrors();

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = IdentitySecurityLogActionConsts.ChangeEmail
        });
    }

    [Authorize]
    public virtual async Task SetProfilePictureAsync(ProfilePictureInput input)
    {
        await SettingManager.SetForUserAsync(CurrentUser.GetId(), AccountSettingNames.ProfilePictureSource, input.Type.ToString());

        var userIdText = CurrentUser.GetId().ToString();

        if (input.Type != ProfilePictureType.Image)
        {
            if (await AccountProfilePictureContainer.ExistsAsync(userIdText))
            {
                await AccountProfilePictureContainer.DeleteAsync(userIdText);
            }
        }
        else
        {
            if (input.ImageContent == null)
            {
                throw new NoImageProvidedException();
            }

            await AccountProfilePictureContainer.SaveAsync(userIdText, input.ImageContent.GetStream(), true);
        }
    }

    public virtual async Task<ProfilePictureSourceDto> GetProfilePictureAsync(Guid id)
    {
        var pictureSource = await SettingManager.GetOrNullForUserAsync(AccountSettingNames.ProfilePictureSource, id);

        if (pictureSource == ProfilePictureType.Gravatar.ToString())
        {
            var user = await UserManager.GetByIdAsync(id);
            var gravatar = $"https://secure.gravatar.com/avatar/{GetGravatarHash(user.Email)}";
            return new ProfilePictureSourceDto
            {
                Type = ProfilePictureType.Gravatar,
                Source = gravatar,
                FileContent = await GetAvatarFromAvatarAsync(gravatar)
            };
        }

        return pictureSource == ProfilePictureType.Image.ToString() && await AccountProfilePictureContainer.ExistsAsync(id.ToString())
            ? new ProfilePictureSourceDto
            {
                Type = ProfilePictureType.Image,
                FileContent = await AccountProfilePictureContainer.GetAllBytesAsync(id.ToString())
            }
            : new ProfilePictureSourceDto
            {
                Type = ProfilePictureType.None,
                FileContent = await GetDefaultAvatarAsync()
            };
    }

    public virtual async Task<IRemoteStreamContent> GetProfilePictureFileAsync(Guid id)
    {
        var picture = await GetProfilePictureAsync(id);
        return new RemoteStreamContent(new MemoryStream(picture.FileContent), contentType: "image/jpeg", disposeStream: true);
    }

    public virtual async Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        return await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor), input.Token)
            ? (await UserManager.GetValidTwoFactorProvidersAsync(user)).ToList()
            : throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }

    public virtual async Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        if (await UserManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, nameof(SignInResult.RequiresTwoFactor), input.Token))
        {
            switch (input.Provider)
            {
                case "Email":
                    {
                        var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                        await AccountEmailer.SendEmailSecurityCodeAsync(user, code);
                        return;
                    }

                case "Phone":
                    {
                        var code = await UserManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultPhoneProvider);
                        await PhoneService.SendSecurityCodeAsync(user, code);
                        return;
                    }

                case "Authenticator":
                    {
                        return;
                    }

                default:
                    throw new UserFriendlyException(L["Volo.Account:UnsupportedTwoFactorProvider"]);
            }
        }

        throw new UserFriendlyException(L["Volo.Account:InvalidUserToken"]);
    }

    [Authorize]
    public async Task<PagedResultDto<IdentitySecurityLogDto>> GetSecurityLogListAsync(GetIdentitySecurityLogListInput input)
    {
        var securityLogs = await SecurityLogRepository.GetListAsync(
            sorting: input.Sorting,
            maxResultCount: input.MaxResultCount,
            skipCount: input.SkipCount,
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.ActionName,
            userId: CurrentUser.GetId(),
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId);

        var totalCount = await SecurityLogRepository.GetCountAsync(
            startTime: input.StartTime,
            endTime: input.EndTime,
            applicationName: input.ApplicationName,
            identity: input.Identity,
            action: input.ActionName,
            userId: CurrentUser.GetId(),
            userName: input.UserName,
            clientId: input.ClientId,
            correlationId: input.CorrelationId);

        var securityLogDtos = ObjectMapper.Map<List<IdentitySecurityLog>, List<IdentitySecurityLogDto>>(securityLogs);
        return new PagedResultDto<IdentitySecurityLogDto>(totalCount, securityLogDtos);
    }

    public virtual async Task CaptchaValidateAsync(string captchaResponse)
    {
        var captchaVersion = await SettingProvider.GetOrNullAsync(AccountSettingNames.Captcha.Version);
        await CaptchaOptions.SetAsync(captchaVersion);

        var captchaValidator = await CaptchaValidatorFactory.CreateAsync();
        await captchaValidator.ValidateAsync(captchaResponse);
    }

    public async Task<ExternalLoginsDto> GetExternalLoginsAsync()
    {
        /*
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        var model = new ExternalLoginsDto
        {
            CurrentLogins = await UserManager.GetLoginsAsync(currentUser)
        };
        model.OtherLogins = (await SignInManager.GetExternalAuthenticationSchemesAsync())
            .Where(auth => model.CurrentLogins.All(ul => auth.Name != ul.LoginProvider))
            .ToList();
        model.ShowRemoveButton = await _userManager.HasPasswordAsync(user) || model.CurrentLogins.Count > 1;

        return (model);
        */

        // TODO: 优化 直接查询 IdentityUserLogin
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
        var userLogins = await UserManager.GetLoginsAsync(currentUser);

        // var schemes = await SignInManager.GetExternalAuthenticationSchemesAsync();
        // var otherLogins = schemes.Where(auth => userLogins.All(ul => auth.Name != ul.LoginProvider)).ToList();
        var schemes = await ExternalProviderSettingsHelper.GetAllAsync();

        var currentLogins = userLogins.Where(ul => schemes.All(auth => auth.Enabled && auth.Name == ul.LoginProvider))
            .Select(p => new UserLoginInfoDto(p.LoginProvider, p.ProviderKey, p.ProviderDisplayName))
            .ToList();
        var otherLogins = schemes.Where(auth => auth.Enabled && userLogins.All(ul => ul.LoginProvider != auth.Name))
            .Select(p => new AuthenticationSchemeDto
            {
                Name = p.Name,

                // DisplayName = L(p.Name)
            }).ToList();
        return new ExternalLoginsDto
        {
            CurrentLogins = currentLogins,
            OtherLogins = otherLogins,
            ShowRemoveButton = !currentUser.PasswordHash.IsNullOrWhiteSpace() || userLogins.Count > 1
        };
    }

    public async Task RemoveLoginAsync(RemoveLoginInput input)
    {
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        var result = await UserManager.RemoveLoginAsync(currentUser, input.LoginProvider, input.ProviderKey);
        if (!result.Succeeded)
        {
            // 解绑失败!
            throw new UserFriendlyException($"Unexpected error occurred removing external login for user with ID '{currentUser.Id}'.");
        }

        // TODO: 判断当前登录是否使用解绑的扩展登录，如果是则让token失效
        // await SignInManager.SignInAsync(currentUser, isPersistent: false);
    }

    public virtual async Task<TwoFactorAuthenticationDto> GetTwoFactorAuthenticationAsync()
    {
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());
        var hasAuthenticator = await UserManager.GetAuthenticatorKeyAsync(currentUser) != null;
        var is2faEnabled = await UserManager.GetTwoFactorEnabledAsync(currentUser);

        return new TwoFactorAuthenticationDto
        {
            HasAuthenticator = hasAuthenticator,
            Is2faEnabled = is2faEnabled,
            CanEnableTwoFactor = hasAuthenticator || is2faEnabled || currentUser.EmailConfirmed || currentUser.PhoneNumberConfirmed,
            RecoveryCodesLeft = await UserManager.CountRecoveryCodesAsync(currentUser),
        };
    }

    public virtual async Task<AuthenticatorInfoDto> GetAuthenticatorInfoAsync()
    {
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        var model = new AuthenticatorInfoDto();

        // TODO: add applicationName to setting
        var applicationName = Configuration["ApplicationName"];
        if (applicationName.IsNullOrWhiteSpace())
        {
            // TODO: Localization 未配置应用程序名称
            throw new UserFriendlyException("The application name is not configured.");
        }

        await LoadSharedKeyAndQrCodeUriAsync(applicationName, currentUser, model);

        return model;
    }

    public virtual async Task<ShowRecoveryCodesDto> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeInput input)
    {
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        // Strip spaces and hypens
        var verificationCode = input.Code.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase);

        var is2faTokenValid = await UserManager.VerifyTwoFactorTokenAsync(
            currentUser, UserManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

        if (!is2faTokenValid)
        {
            throw new UserFriendlyException("This TwoFactor Token is not valid.");
        }

        // await UserManager.SetTwoFactorEnabledAsync(currentUser, true);
        Logger.LogInformation("User with ID {UserId} has enabled 2FA with an authenticator app.", currentUser.Id);
        var recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(currentUser, 10);

        return new ShowRecoveryCodesDto { RecoveryCodes = recoveryCodes.ToArray() };
    }

    public virtual async Task ResetAuthenticatorAsync()
    {
        // var user = await _userManager.GetUserAsync(User);
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        await UserManager.SetTwoFactorEnabledAsync(currentUser, false);
        await UserManager.ResetAuthenticatorKeyAsync(currentUser);
        Logger.LogInformation("User with id '{UserId}' has reset their authentication app key.", currentUser.Id);

        // return RedirectToAction(nameof(EnableAuthenticator));
    }

    public virtual async Task<ShowRecoveryCodesDto> GenerateRecoveryCodesAsync()
    {
        // var user = await _userManager.GetUserAsync(User);
        var currentUser = await UserManager.GetByIdAsync(CurrentUser.GetId());

        if (!currentUser.TwoFactorEnabled)
        {
            throw new ApplicationException($"Cannot generate recovery codes for user with ID '{currentUser.Id}' as they do not have 2FA enabled.");
        }

        var recoveryCodes = await UserManager.GenerateNewTwoFactorRecoveryCodesAsync(currentUser, 10);
        Logger.LogInformation("User with ID {UserId} has generated new 2FA recovery codes.", currentUser.Id);

        return new ShowRecoveryCodesDto { RecoveryCodes = recoveryCodes.ToArray() };
    }

    protected virtual string GetGravatarHash(string emailAddress)
    {
        var encodedPassword = new UTF8Encoding().GetBytes(emailAddress);

        var hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);

        return BitConverter.ToString(hash)
            .Replace("-", string.Empty, StringComparison.OrdinalIgnoreCase)
            .ToLower(CultureInfo.CurrentCulture);
    }

    protected virtual async Task<IdentityUser> GetUserByEmailAsync(string email)
    {
        var user = await UserManager.FindByEmailAsync(email);
        return user ?? throw new UserFriendlyException(L["Volo.Account:InvalidEmailAddress", email]);
    }

    protected virtual async Task CheckSelfRegistrationAsync()
    {
        if (!await SettingProvider.IsTrueAsync(AccountSettingNames.IsSelfRegistrationEnabled))
        {
            throw new UserFriendlyException(L["Volo.Account:SelfRegistrationDisabled"]);
        }
    }

    protected virtual void CheckPhoneNumber(IdentityUser user)
    {
        if (string.IsNullOrEmpty(user.PhoneNumber))
        {
            throw new BusinessException("Volo.Account:PhoneNumberEmpty");
        }
    }

    protected virtual async Task CheckIfPhoneNumberConfirmationEnabledAsync()
    {
        if (!await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.EnablePhoneNumberConfirmation))
        {
            throw new BusinessException("Volo.Account:PhoneNumberConfirmationDisabled");
        }
    }

    protected virtual async Task<bool> UseCaptchaOnRegistrationAsync()
    {
        return await SettingProvider.IsTrueAsync(AccountSettingNames.Captcha.UseCaptchaOnRegistration);
    }

    // TODO: cache byte[]
    protected virtual async Task<byte[]> GetAvatarFromAvatarAsync(string url)
    {
        var httpClientFactory = LazyServiceProvider.LazyGetRequiredService<IHttpClientFactory>();
        using var httpclient = httpClientFactory.CreateClient("GravatarHttpClient");
        var responseMessage = await httpclient.GetAsync(url);
        return await responseMessage.Content.ReadAsByteArrayAsync();
    }

    // TODO: cache byte[]
    protected virtual async Task<byte[]> GetDefaultAvatarAsync()
    {
        var virtualFileProvider = LazyServiceProvider.LazyGetRequiredService<IVirtualFileProvider>();
        using var stream = virtualFileProvider.GetFileInfo("/X/Abp/Account/ProfilePictures/avatar.jpg").CreateReadStream();
        return await stream.GetAllBytesAsync();
    }

    protected async Task LoadSharedKeyAndQrCodeUriAsync(string applicationName, IdentityUser user, AuthenticatorInfoDto model)
    {
        var unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(unformattedKey))
        {
            await UserManager.ResetAuthenticatorKeyAsync(user);
            unformattedKey = await UserManager.GetAuthenticatorKeyAsync(user);
        }

        model.SharedKey = FormatKey(unformattedKey);
        model.AuthenticatorUri = GenerateQrCodeUri(applicationName, user.Email, unformattedKey);
    }

    public static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        var currentPosition = 0;
        while (currentPosition + 4 < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition, 4)).Append(' ');
            currentPosition += 4;
        }

        if (currentPosition < unformattedKey.Length)
        {
            result.Append(unformattedKey.AsSpan(currentPosition));
        }

        return result.ToString().ToLowerInvariant();
    }

    public static string GenerateQrCodeUri(string applicationName, string email, string unformattedKey)
    {
        return string.Format(
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
            UrlEncoder.Default.Encode(applicationName),
            UrlEncoder.Default.Encode(email),
            unformattedKey);
    }
}
