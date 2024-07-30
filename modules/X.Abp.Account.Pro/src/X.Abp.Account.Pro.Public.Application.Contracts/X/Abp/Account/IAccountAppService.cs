// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

using X.Abp.Account.Dtos;
using X.Abp.Identity;

namespace X.Abp.Account;

public interface IAccountAppService : IApplicationService
{
    Task<IdentityUserDto> RegisterAsync(RegisterDto input);

    Task SendPasswordResetCodeAsync(SendPasswordResetCodeDto input);

    Task ResetPasswordAsync(ResetPasswordDto input);

    Task<IdentityUserConfirmationStateDto> GetConfirmationStateAsync(Guid id);

    Task SendPhoneNumberConfirmationTokenAsync(SendPhoneNumberConfirmationTokenDto input);

    Task SendEmailConfirmationTokenAsync(SendEmailConfirmationTokenDto input);

    Task ConfirmPhoneNumberAsync(ConfirmPhoneNumberInput input);

    Task ConfirmEmailAsync(ConfirmEmailInput input);

    Task SetProfilePictureAsync(ProfilePictureInput input);

    Task<ProfilePictureSourceDto> GetProfilePictureAsync(Guid id);

    Task<IRemoteStreamContent> GetProfilePictureFileAsync(Guid id);

    Task<List<string>> GetTwoFactorProvidersAsync(GetTwoFactorProvidersInput input);

    Task SendTwoFactorCodeAsync(SendTwoFactorCodeInput input);

    Task<PagedResultDto<IdentitySecurityLogDto>> GetSecurityLogListAsync(GetIdentitySecurityLogListInput input);

    Task CaptchaValidateAsync(string captchaResponse);

    Task<ExternalLoginsDto> GetExternalLoginsAsync();

    Task RemoveLoginAsync(RemoveLoginInput input);

    Task<TwoFactorAuthenticationDto> GetTwoFactorAuthenticationAsync();

    Task<AuthenticatorInfoDto> GetAuthenticatorInfoAsync();

    Task<ShowRecoveryCodesDto> VerifyAuthenticatorCodeAsync(VerifyAuthenticatorCodeInput input);

    Task ResetAuthenticatorAsync();

    Task<ShowRecoveryCodesDto> GenerateRecoveryCodesAsync();
}
