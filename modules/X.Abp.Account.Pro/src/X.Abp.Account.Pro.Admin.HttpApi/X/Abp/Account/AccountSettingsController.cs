// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.Account;

[RemoteService(Name = AbpAccountAdminRemoteServiceConsts.RemoteServiceName)]
[Area(AbpAccountAdminRemoteServiceConsts.ModuleName)]
[Route("api/account-admin/settings")]
public class AccountSettingsController : AbpController, IAccountSettingsAppService
{
    protected IAccountSettingsAppService AccountSettingsAppService { get; }

    public AccountSettingsController(IAccountSettingsAppService accountSettingsAppService)
    {
        AccountSettingsAppService = accountSettingsAppService;
    }

    [HttpGet]
    public virtual async Task<AccountSettingsDto> GetAsync()
    {
        return await AccountSettingsAppService.GetAsync();
    }

    [HttpPut]
    public virtual async Task UpdateAsync(AccountSettingsDto input)
    {
        await AccountSettingsAppService.UpdateAsync(input);
    }

    [HttpGet]
    [Route("two-factor")]
    public virtual async Task<AccountTwoFactorSettingsDto> GetTwoFactorAsync()
    {
        return await AccountSettingsAppService.GetTwoFactorAsync();
    }

    [HttpPut]
    [Route("two-factor")]
    public virtual async Task UpdateTwoFactorAsync(AccountTwoFactorSettingsDto input)
    {
        await AccountSettingsAppService.UpdateTwoFactorAsync(input);
    }

    [HttpGet]
    [Route("captcha")]
    public virtual async Task<AccountCaptchaSettingsDto> GetRecaptchaAsync()
    {
        return await AccountSettingsAppService.GetRecaptchaAsync();
    }

    [HttpPut]
    [Route("captcha")]
    public virtual async Task UpdateRecaptchaAsync(AccountCaptchaSettingsDto input)
    {
        await AccountSettingsAppService.UpdateRecaptchaAsync(input);
    }

    [HttpGet]
    [Route("external-provider")]
    public virtual async Task<AccountExternalProviderSettingsDto> GetExternalProviderAsync()
    {
        return await AccountSettingsAppService.GetExternalProviderAsync();
    }

    [HttpPut]
    [Route("external-provider")]
    public virtual async Task UpdateExternalProviderAsync(List<UpdateExternalProviderDto> input)
    {
        await AccountSettingsAppService.UpdateExternalProviderAsync(input);
    }
}
