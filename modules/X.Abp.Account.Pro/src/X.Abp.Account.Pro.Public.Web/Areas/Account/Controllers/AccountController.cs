// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Identity;
using Volo.Abp.Identity.AspNetCore;
using Volo.Abp.Security.Claims;
using Volo.Abp.Users;
using Volo.Abp.Validation;

using X.Abp.Account.Dtos;
using X.Abp.Account.Public.Web.Areas.Account.Controllers.Models;
using X.Abp.Account.Public.Web.ExternalProviders;
using X.Abp.Identity;

using IdentityUser = Volo.Abp.Identity.IdentityUser;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;
using UserLoginInfo = X.Abp.Account.Public.Web.Areas.Account.Controllers.Models.UserLoginInfo;

namespace X.Abp.Account.Public.Web.Areas.Account.Controllers;

[RemoteService(Name = AbpAccountPublicRemoteServiceConsts.RemoteServiceName)]
[Controller]
[ControllerName("Login")]
[Area(AbpAccountPublicRemoteServiceConsts.ModuleName)]
[Route("api/account")]
public class AccountController : AccountControllerBase
{
    protected IConfiguration Configuration => LazyServiceProvider.LazyGetRequiredService<IConfiguration>();

    protected ITokenGeneratorProvider TokenGeneratorProvider => LazyServiceProvider.LazyGetRequiredService<ITokenGeneratorProvider>();

    protected IScanCodeLoginProvider ScanCodeLoginProvider => LazyServiceProvider.LazyGetRequiredService<IScanCodeLoginProvider>();

    protected SignInManager<IdentityUser> SignInManager => LazyServiceProvider.LazyGetRequiredService<SignInManager<IdentityUser>>();

    protected IdentityUserManager UserManager => LazyServiceProvider.LazyGetRequiredService<IdentityUserManager>();

    protected IdentitySecurityLogManager IdentitySecurityLogManager => LazyServiceProvider.LazyGetRequiredService<IdentitySecurityLogManager>();

    protected IIdentityLinkUserAppService IdentityLinkUserAppService => LazyServiceProvider.LazyGetRequiredService<IIdentityLinkUserAppService>();

    protected ICurrentPrincipalAccessor CurrentPrincipalAccessor => LazyServiceProvider.LazyGetRequiredService<ICurrentPrincipalAccessor>();

    protected IOptions<IdentityOptions> IdentityOptions => LazyServiceProvider.LazyGetRequiredService<IOptions<IdentityOptions>>();

    /// <summary>
    /// 登录【Cookies】
    /// </summary>
    /// <param name="login">登录信息</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [HttpPost]
    [Route("login")]
    public virtual async Task<AbpLoginResult> LoginAsync(UserLoginInfo login)
    {
        ValidateLoginInfo(login);

        await ReplaceEmailToUsernameOfInputIfNeeds(login);

        var signInResult = await SignInManager.PasswordSignInAsync(
            login.UserNameOrEmailAddress,
            login.Password,
            login.RememberMe,
            true);

        await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
        {
            Identity = IdentitySecurityLogIdentityConsts.Identity,
            Action = signInResult.ToIdentitySecurityLogAction(),
            UserName = login.UserNameOrEmailAddress
        });

        return GetAbpLoginResult(signInResult);
    }

    [HttpPost]
    [Route("link-login")]
    public virtual async Task<AbpLoginResult> LinkLoginAsync(LinkUserLoginInfo login)
    {
        if (login.LinkUserId == CurrentUser.Id && login.LinkTenantId == CurrentTenant.Id)
        {
            return new AbpLoginResult(LoginResultType.Success);
        }

        if (await IdentityLinkUserAppService.IsLinkedAsync(new IsLinkedInput
        {
            UserId = login.LinkUserId,
            TenantId = login.LinkTenantId
        }))
        {
            var isPersistent = (await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme))?.Properties?.IsPersistent ?? false;
            await SignInManager.SignOutAsync();
            using (CurrentTenant.Change(login.LinkTenantId))
            {
                var targetUser = await UserManager.GetByIdAsync(login.LinkUserId);
                await SignInManager.SignInAsync(targetUser, isPersistent);
            }

            return new AbpLoginResult(LoginResultType.Success);
        }

        return new AbpLoginResult(LoginResultType.NotLinked);
    }

    [HttpPost]
    [Route("generate-qrcode")]
    [AllowAnonymous]
    public virtual async Task<GenerateQrCodeResult> GenerateQrCodeAsync()
    {
        string qrCodeKey = await ScanCodeLoginProvider.GenerateQrCodeAsync();

        return new GenerateQrCodeResult
        {
            QrCodeKey = qrCodeKey,
            QrCodeType = "ScanCodeLogin",
            Url = Url.PageLink("/Account/ScanCodeLogin", values: new { QrCodeKey = qrCodeKey })
        };
    }

    [HttpPost]
    [Route("check-qrcode")]
    [AllowAnonymous]
    public virtual async Task<LoginQrCode> CheckQrCodeAsync(string qrCodeKey)
    {
        return await ScanCodeLoginProvider.CheckQrCodeAsync(qrCodeKey);
    }

    /// <summary>
    /// 扫码登录
    /// </summary>
    /// <param name="qrCodeKey">qrCodeKey</param>
    /// <returns>IActionResult</returns>
    [HttpGet]
    [Route("scancode-login")]
    [Authorize]
    public virtual async Task<LoginQrCode> ScanCodeLoginAsync(string qrCodeKey)
    {
        return await ScanCodeLoginProvider.ScanCodeAsync(qrCodeKey, CurrentUser.GetId(), CurrentTenant.Id);
    }

    /// <summary>
    /// 扫码登录确认
    /// </summary>
    /// <param name="qrCodeKey">qrCodeKey</param>
    [HttpPost]
    [Route("scancode-login-confirm")]
    [Authorize]
    public virtual async Task<bool> ScanCodeLoginConfirmAsync(string qrCodeKey)
    {
        using (CurrentTenant.Change(CurrentTenant.Id))
        {
            var user = await UserManager.GetByIdAsync(CurrentUser.GetId());
            var token = await UserManager.GenerateUserTokenAsync(
                user,
                ScanCodeUserTokenProviderConsts.ScanCodeUserTokenProviderName,
                ScanCodeUserTokenProviderConsts.ScanCodeUserLoginTokenPurpose);

            var qrCodeInfo = await ScanCodeLoginProvider.ConfirmLoginAsync(qrCodeKey, token);

            if (qrCodeInfo.QrCodeStatus != QrCodeStatus.Confirmed)
            {
                return await Task.FromResult(false);
            }

            return await Task.FromResult(true);
        }
    }

    /// <summary>
    /// 扫码登录
    /// </summary>
    /// <param name="login">login</param>
    [HttpPost]
    [Route("scancode-login")]
    [AllowAnonymous]
    public virtual async Task<IActionResult> ScanCodeLoginAsync(ScanCodeLoginInfo login)
    {
        var scanCodeInfo = await ScanCodeLoginProvider.GetQrCodeAsync(login.QrCodeKey);
        if (scanCodeInfo == null)
        {
            Logger.LogWarning("ScanCode login info is not available");
            return Redirect(QueryHelpers.AddQueryString(login.ReturnUrl, new Dictionary<string, string>()
            {
                ["error"] = "ScanCode login info is not available"
            }));
        }

        Logger.LogInformation($"CurrentTenant:{scanCodeInfo.TenantId}");

        using (CurrentTenant.Change(scanCodeInfo.TenantId))
        {
            var user = await UserManager.GetByIdAsync(scanCodeInfo.UserId.Value);
            var verify = await UserManager.VerifyUserTokenAsync(
                user,
                ScanCodeUserTokenProviderConsts.ScanCodeUserTokenProviderName,
                ScanCodeUserTokenProviderConsts.ScanCodeUserLoginTokenPurpose,
                login.ScanCodeToken);

            if (!verify)
            {
                Logger.LogWarning("InvalidScanCodeToken");
                return Redirect(QueryHelpers.AddQueryString(login.ReturnUrl, new Dictionary<string, string>()
                {
                    ["error"] = "Invalid ScanCodeToken"
                }));
            }
        }

        var result = await TokenGeneratorProvider.CreateScanCodeLoginAccessTokenAsync(scanCodeInfo.UserId.Value, scanCodeInfo.TenantId, login.ClientId, login.ClientSecret, login.Scope);

        return result.IsError
        ? Redirect(QueryHelpers.AddQueryString(login.ReturnUrl, new Dictionary<string, string>()
        {
            ["error"] = result.Error
        }))
        : Redirect(QueryHelpers.AddQueryString(login.ReturnUrl, new Dictionary<string, string>()
        {
            ["access_token"] = result.AccessToken,
            ["refresh_token"] = result.RefreshToken,
            ["expires_in"] = result.ExpiresIn.ToString(),
            ["token_type"] = result.TokenType
        }));
    }

    /// <summary>
    /// SPA第三方账号登录
    /// </summary>
    /// <param name="provider">第三方提供者</param>
    /// <param name="clientId">客户端Id</param>
    /// <param name="clientSecret">客户端Secret</param>
    /// <param name="scope">scope</param>
    /// <param name="returnUrl">登录成功回调地址</param>
    /// <returns>IActionResult</returns>
    [HttpPost]
    [Route("spa-external-login")]
    [AllowAnonymous]
    public virtual async Task<IActionResult> SpaExternalLoginAsync(string provider, string clientId, string clientSecret, string scope, string returnUrl)
    {
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(scope) || string.IsNullOrWhiteSpace(returnUrl))
        {
            Logger.LogWarning("The parameter is incorrect");
            return Redirect(QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string>()
            {
                ["error"] = "The parameter is incorrect"
            }));
        }

        var tenantId = CurrentTenant.Id;
        Logger.LogInformation($"CurrentTenant:{tenantId}");

        var redirectUrl = Url.Page("/Account/SpaExternalLogin", pageHandler: "Callback", values: new { returnUrl, tenantId, clientId, clientSecret, scope });

        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    /// <summary>
    /// SPA绑定第三方绑定
    /// </summary>
    /// <param name="provider">第三方提供者</param>
    /// <param name="returnUrl">绑定成功回调地址</param>
    /// <returns>IActionResult</returns>
    [HttpPost]
    [Route("spa-external-login-bind")]
    [Authorize]
    public virtual async Task<IActionResult> SpaExternalLoginBindAsync(string provider, string returnUrl)
    {
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(returnUrl))
        {
            Logger.LogWarning("The parameter is incorrect");
            return Redirect(QueryHelpers.AddQueryString(returnUrl, new Dictionary<string, string>()
            {
                ["error"] = "The parameter is incorrect"
            }));
        }

        var tenantId = CurrentTenant.Id;
        Logger.LogInformation($"CurrentTenant:{tenantId}");
        var userId = CurrentUser.GetId();
        Logger.LogInformation($"CurrentUser:{userId}");

        var redirectUrl = Url.Page("/Account/SpaExternalLoginBind", pageHandler: "Callback", values: new { returnUrl, userId, tenantId });

        var properties = SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, userId.ToString());
        properties.Items["scheme"] = provider;

        return await Task.FromResult(Challenge(properties, provider));
    }

    [HttpGet]
    [Route("logout")]
    public virtual async Task LogoutAsync()
    {
        if (CurrentUser.IsAuthenticated)
        {
            await IdentitySecurityLogManager.SaveAsync(new IdentitySecurityLogContext
            {
                Identity = IdentitySecurityLogIdentityConsts.Identity,
                Action = IdentitySecurityLogActionConsts.Logout
            });
        }

        await SignInManager.SignOutAsync();
    }

    [HttpPost]
    [Route("check-password")]
    public virtual async Task<AbpLoginResult> CheckPasswordAsync(UserLoginInfo login)
    {
        ValidateLoginInfo(login);

        await ReplaceEmailToUsernameOfInputIfNeeds(login);

        var identityUser = await UserManager.FindByNameAsync(login.UserNameOrEmailAddress);

        identityUser ??= await UserManager.FindByEmailAsync(login.UserNameOrEmailAddress);

        await IdentityOptions.SetAsync();
        return GetAbpLoginResult(await SignInManager.CheckPasswordSignInAsync(identityUser, login.Password, true));
    }

    private static AbpLoginResult GetAbpLoginResult(SignInResult result)
    {
        return result.IsLockedOut
            ? new AbpLoginResult(LoginResultType.LockedOut)
            : result.RequiresTwoFactor
            ? new AbpLoginResult(LoginResultType.RequiresTwoFactor)
            : result.IsNotAllowed
            ? new AbpLoginResult(LoginResultType.NotAllowed)
            : !result.Succeeded
            ? new AbpLoginResult(LoginResultType.InvalidUserNameOrPassword)
            : new AbpLoginResult(LoginResultType.Success);
    }

    protected virtual void ValidateLoginInfo(UserLoginInfo login)
    {
        if (login == null)
        {
            throw new ArgumentException(nameof(login));
        }

        if (login.UserNameOrEmailAddress.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(login.UserNameOrEmailAddress));
        }

        if (login.Password.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(login.Password));
        }
    }

    protected virtual async Task ReplaceEmailToUsernameOfInputIfNeeds(UserLoginInfo login)
    {
        if (!ValidationHelper.IsValidEmailAddress(login.UserNameOrEmailAddress))
        {
            return;
        }

        var userByUsername = await UserManager.FindByNameAsync(login.UserNameOrEmailAddress);
        if (userByUsername != null)
        {
            return;
        }

        var userByEmail = await UserManager.FindByEmailAsync(login.UserNameOrEmailAddress);
        if (userByEmail == null)
        {
            return;
        }

        login.UserNameOrEmailAddress = userByEmail.UserName;
    }

    protected virtual async Task<TokenGeneratorResult> SpaExternalLoginSignInAsync(Guid? tenantId, string loginProvider, string providerKey, string clientId, string clientSecret, string scope)
    {
        // Cleanup cookie
        await SignInManager.SignOutAsync();

        return await TokenGeneratorProvider.CreateSpaExternalLoginAccessTokenAsync(loginProvider, providerKey, tenantId, clientId, clientSecret, scope);
    }

    // TODO: 用户不存在是否需要创建
    protected virtual async Task<IdentityUser> CreateExternalUserAsync(ExternalLoginInfo info)
    {
        await IdentityOptions.SetAsync();

        var emailAddress = info.Principal.FindFirstValue(AbpClaimTypes.Email);

        var user = new IdentityUser(GuidGenerator.Create(), emailAddress, emailAddress, CurrentTenant.Id);

        CheckIdentityErrors(await UserManager.CreateAsync(user));
        CheckIdentityErrors(await UserManager.SetEmailAsync(user, emailAddress));
        CheckIdentityErrors(await UserManager.AddLoginAsync(user, info));
        CheckIdentityErrors(await UserManager.AddDefaultRolesAsync(user));

        user.Name = info.Principal.FindFirstValue(AbpClaimTypes.Name);
        user.Surname = info.Principal.FindFirstValue(AbpClaimTypes.SurName);

        var phoneNumber = info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumber);
        if (!phoneNumber.IsNullOrWhiteSpace())
        {
            var phoneNumberConfirmed = string.Equals(info.Principal.FindFirstValue(AbpClaimTypes.PhoneNumberVerified), "true", StringComparison.OrdinalIgnoreCase);
            user.SetPhoneNumber(phoneNumber, phoneNumberConfirmed);
        }

        CheckIdentityErrors(await UserManager.UpdateAsync(user));

        return user;
    }

    protected virtual void CheckIdentityErrors(IdentityResult identityResult)
    {
        if (!identityResult.Succeeded)
        {
            throw new UserFriendlyException("Operation failed: " + identityResult.Errors.Select(e => $"[{e.Code}] {e.Description}").JoinAsString(", "));
        }

        // identityResult.CheckIdentityErrors(LocalizationManager); //TODO: Get from old Abp
    }
}
