// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Account.Dtos;

namespace X.Abp.Account;

[RemoteService(Name = AbpAccountPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AbpAccountPublicRemoteServiceConsts.ModuleName)]
[ControllerName("User")]
[Route("api/account/link-user")]
public class IdentityLinkUserController : AbpControllerBase, IIdentityLinkUserAppService
{
    protected IIdentityLinkUserAppService IdentityLinkUserAppService { get; }

    public IdentityLinkUserController(IIdentityLinkUserAppService identityLinkUserAppService)
    {
        IdentityLinkUserAppService = identityLinkUserAppService;
    }

    [HttpPost]
    [Route("link")]
    public Task LinkAsync(LinkUserInput input)
    {
        return IdentityLinkUserAppService.LinkAsync(input);
    }

    [HttpPost]
    [Route("unlink")]
    public Task UnlinkAsync(UnLinkUserInput input)
    {
        return IdentityLinkUserAppService.UnlinkAsync(input);
    }

    [HttpPost]
    [Route("is-linked")]
    public Task<bool> IsLinkedAsync(IsLinkedInput input)
    {
        return IdentityLinkUserAppService.IsLinkedAsync(input);
    }

    [HttpPost]
    [Route("generate-link-token")]
    public Task<string> GenerateLinkTokenAsync()
    {
        return IdentityLinkUserAppService.GenerateLinkTokenAsync();
    }

    [HttpPost]
    [Route("verify-link-token")]
    public Task<bool> VerifyLinkTokenAsync(VerifyLinkTokenInput input)
    {
        return IdentityLinkUserAppService.VerifyLinkTokenAsync(input);
    }

    [HttpPost]
    [Route("generate-link-login-token")]
    public Task<string> GenerateLinkLoginTokenAsync()
    {
        return IdentityLinkUserAppService.GenerateLinkLoginTokenAsync();
    }

    [HttpPost]
    [Route("verify-link-login-token")]
    public Task<bool> VerifyLinkLoginTokenAsync(VerifyLinkLoginTokenInput input)
    {
        return IdentityLinkUserAppService.VerifyLinkLoginTokenAsync(input);
    }

    [HttpGet]
    public Task<ListResultDto<LinkUserDto>> GetAllListAsync()
    {
        return IdentityLinkUserAppService.GetAllListAsync();
    }
}
