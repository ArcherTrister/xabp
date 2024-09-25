// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Account.Dtos;

namespace X.Abp.Account;

[RemoteService(Name = AbpAccountPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AbpAccountPublicRemoteServiceConsts.ModuleName)]
[ControllerName("Profile")]
[Route("/api/account/my-profile")]
public class ProfileController : AbpControllerBase, IProfileAppService
{
    protected IProfileAppService ProfileAppService { get; }

    public ProfileController(IProfileAppService profileAppService)
    {
        ProfileAppService = profileAppService;
    }

    [HttpGet]
    public virtual Task<ProfileDto> GetAsync()
    {
        return ProfileAppService.GetAsync();
    }

    [HttpPut]
    public virtual Task<ProfileDto> UpdateAsync(UpdateProfileDto input)
    {
        return ProfileAppService.UpdateAsync(input);
    }

    [HttpPost]
    [Route("change-password")]
    public virtual Task ChangePasswordAsync(ChangePasswordInput input)
    {
        return ProfileAppService.ChangePasswordAsync(input);
    }

    [HttpGet]
    [Route("two-factor-enabled")]
    public virtual Task<bool> GetTwoFactorEnabledAsync()
    {
        return ProfileAppService.GetTwoFactorEnabledAsync();
    }

    [HttpPost]
    [Route("set-two-factor-enabled")]
    public virtual Task SetTwoFactorEnabledAsync(bool enabled)
    {
        return ProfileAppService.SetTwoFactorEnabledAsync(enabled);
    }

    [HttpGet]
    [Route("can-enable-two-factor")]
    public virtual Task<bool> CanEnableTwoFactorAsync()
    {
        return ProfileAppService.CanEnableTwoFactorAsync();
    }

    [HttpGet]
    [Route("timezones")]
    public virtual Task<List<NameValue>> GetTimezonesAsync()
    {
        return ProfileAppService.GetTimezonesAsync();
    }
}
