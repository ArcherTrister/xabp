// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Content;

using X.Abp.Account.Dtos;
using X.Abp.Identity;

namespace X.Abp.Account;

[RemoteService(Name = AbpAccountPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AbpAccountPublicRemoteServiceConsts.ModuleName)]
[Route("api/account")]
public class AccountController : AbpControllerBase, IAccountAppService
{
    protected IAccountAppService AccountAppService { get; }

    public AccountController(IAccountAppService accountAppService)
    {
        AccountAppService = accountAppService;
    }

    [HttpPost]
    [Route("register")]
    public virtual async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        return await AccountAppService.RegisterAsync(input);
    }

    [HttpPost]
    [Route("send-password-reset-code")]
    public virtual Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input)
    {
        return AccountAppService.SendPasswordResetCodeAsync(input);
    }

    [HttpPost]
    [Route("reset-password")]
    public virtual Task ResetPasswordAsync(ResetPasswordDto input)
    {
        return AccountAppService.ResetPasswordAsync(input);
    }

    [HttpGet]
    [Route("confirmation-state")]
    public Task<IdentityUserConfirmationStateDto> GetConfirmationStateAsync(Guid id)
    {
        return AccountAppService.GetConfirmationStateAsync(id);
    }

    [HttpPost]
    [Route("send-phone-number-confirmation-token")]
    public Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input)
    {
        return AccountAppService.SendPhoneNumberConfirmationTokenAsync(input);
    }

    [HttpPost]
    [Route("send-email-confirmation-token")]
    public Task SendEmailConfirmationTokenAsync(SendEmailConfirmationTokenDto input)
    {
        return AccountAppService.SendEmailConfirmationTokenAsync(input);
    }

    [HttpPost]
    [Route("confirm-phone-number")]
    public Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input)
    {
        return AccountAppService.ConfirmPhoneNumberAsync(input);
    }

    [HttpPost]
    [Route("confirm-email")]
    public Task ConfirmEmailAsync(ConfirmEmailInput input)
    {
        return AccountAppService.ConfirmEmailAsync(input);
    }

    [Authorize]
    [HttpPost]
    [Route("profile-picture")]
    public virtual async Task SetProfilePictureAsync(ProfilePictureInput input)
    {
        await AccountAppService.SetProfilePictureAsync(input);
    }

    [HttpGet]
    [Route("profile-picture/{id}")]
    public virtual async Task<ProfilePictureSourceDto> GetProfilePictureAsync(Guid id)
    {
        return await AccountAppService.GetProfilePictureAsync(id);
    }

    [HttpGet]
    [Route("two-factor-providers")]
    public virtual Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input)
    {
        return AccountAppService.GetTwoFactorProvidersAsync(input);
    }

    [HttpPost]
    [Route("send-two-factor-code")]
    public virtual Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input)
    {
        return AccountAppService.SendTwoFactorCodeAsync(input);
    }

    [HttpGet]
    [Route("security-logs")]
    public Task<PagedResultDto<IdentitySecurityLogDto>> GetSecurityLogListAsync([FromQuery] GetIdentitySecurityLogListInput input)
    {
        return AccountAppService.GetSecurityLogListAsync(input);
    }

    [HttpGet]
    [Route("profile-picture-file/{id}")]
    public virtual async Task<IRemoteStreamContent> GetProfilePictureFileAsync(Guid id)
    {
        return await AccountAppService.GetProfilePictureFileAsync(id);
    }

    [HttpGet]
    [Route("captcha-validate")]
    public virtual async Task CaptchaValidateAsync(string captchaResponse)
    {
        await AccountAppService.CaptchaValidateAsync(captchaResponse);
    }

    [HttpGet]
    [Route("external-logins")]
    public virtual Task<ExternalLoginsDto> GetExternalLoginsAsync()
    {
        return AccountAppService.GetExternalLoginsAsync();
    }

    [HttpPost]
    [Route("remove-login")]
    public virtual Task RemoveLoginAsync(RemoveLoginInput input)
    {
        return AccountAppService.RemoveLoginAsync(input);
    }

    [HttpGet]
    [Route("two-factor-authentication")]
    public virtual Task<TwoFactorAuthenticationDto> GetTwoFactorAuthenticationAsync()
    {
        return AccountAppService.GetTwoFactorAuthenticationAsync();
    }

    [HttpGet]
    [Route("authenticator-info")]
    public virtual Task<AuthenticatorInfoDto> GetAuthenticatorInfoAsync()
    {
        return AccountAppService.GetAuthenticatorInfoAsync();
    }

    [HttpPost]
    [Route("verify-authenticator-code")]
    public virtual Task<ShowRecoveryCodesDto> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeInput input)
    {
        return AccountAppService.VerifyAuthenticatorCodeAsync(input);
    }

    [HttpPost]
    [Route("reset-authenticator")]
    public virtual Task ResetAuthenticatorAsync()
    {
        return AccountAppService.ResetAuthenticatorAsync();
    }

    [HttpPost]
    [Route("generate-recovery-codes")]
    public virtual Task<ShowRecoveryCodesDto> GenerateRecoveryCodesAsync()
    {
        return AccountAppService.GenerateRecoveryCodesAsync();
    }
}
