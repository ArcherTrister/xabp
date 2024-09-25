// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.Identity;

[RemoteService(Name = AbpIdentityProRemoteServiceConsts.RemoteServiceName)]
[Area(AbpIdentityProRemoteServiceConsts.ModuleName)]
[ControllerName("Settings")]
[Route("api/identity/settings")]
public class IdentitySettingsController : AbpControllerBase, IIdentitySettingsAppService
{
    protected IIdentitySettingsAppService IdentitySettingsAppService { get; }

    public IdentitySettingsController(IIdentitySettingsAppService identitySettingsAppService)
    {
        IdentitySettingsAppService = identitySettingsAppService;
    }

    [HttpGet]
    public virtual Task<IdentitySettingsDto> GetAsync()
    {
        return IdentitySettingsAppService.GetAsync();
    }

    [HttpPut]
    public virtual Task UpdateAsync(IdentitySettingsDto input)
    {
        return IdentitySettingsAppService.UpdateAsync(input);
    }

    [HttpGet]
    [Route("ldap")]
    public virtual async Task<IdentityLdapSettingsDto> GetLdapAsync()
    {
        return await IdentitySettingsAppService.GetLdapAsync();
    }

    [HttpPut]
    [Route("ldap")]
    public virtual async Task UpdateLdapAsync(IdentityLdapSettingsDto input)
    {
        await IdentitySettingsAppService.UpdateLdapAsync(input);
    }

    [HttpGet]
    [Route("oauth")]
    public virtual Task<IdentityOAuthSettingsDto> GetOAuthAsync()
    {
        return IdentitySettingsAppService.GetOAuthAsync();
    }

    [HttpPut]
    [Route("oauth")]
    public virtual Task UpdateOAuthAsync(IdentityOAuthSettingsDto input)
    {
        return IdentitySettingsAppService.UpdateOAuthAsync(input);
    }

    [HttpGet]
    [Route("session")]
    public virtual Task<IdentitySessionSettingsDto> GetSessionAsync()
    {
        return IdentitySettingsAppService.GetSessionAsync();
    }

    [HttpPut]
    [Route("session")]
    public virtual Task UpdateSessionAsync(IdentitySessionSettingsDto input)
    {
        return IdentitySettingsAppService.UpdateSessionAsync(input);
    }
}
