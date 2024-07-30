// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace X.Abp.Account;

public interface IAccountSettingsAppService : IApplicationService
{
    Task<AccountSettingsDto> GetAsync();

    Task UpdateAsync(AccountSettingsDto input);

    Task<AccountTwoFactorSettingsDto> GetTwoFactorAsync();

    Task UpdateTwoFactorAsync(AccountTwoFactorSettingsDto input);

    Task<AccountCaptchaSettingsDto> GetRecaptchaAsync();

    Task UpdateRecaptchaAsync(AccountCaptchaSettingsDto input);

    Task<AccountExternalProviderSettingsDto> GetExternalProviderAsync();

    Task UpdateExternalProviderAsync(List<UpdateExternalProviderDto> input);
}
