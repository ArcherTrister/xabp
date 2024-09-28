// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Account.Dtos;
using X.Abp.Identity;

namespace X.Abp.Account;

[RemoteService(Name = AbpAccountPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AbpAccountPublicRemoteServiceConsts.ModuleName)]
[Route("api/account/sessions")]
[ControllerName("Sessions")]
public class AccountSessionController : AbpControllerBase, IAccountSessionAppService
{
    protected IAccountSessionAppService AccountSessionAppService { get; }

    public AccountSessionController(IAccountSessionAppService accountSessionAppService)
    {
        AccountSessionAppService = accountSessionAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<IdentitySessionDto>> GetListAsync(GetAccountIdentitySessionListInput input)
    {
        return AccountSessionAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<IdentitySessionDto> GetAsync(Guid id)
    {
        return AccountSessionAppService.GetAsync(id);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task RevokeAsync(Guid id)
    {
        return AccountSessionAppService.RevokeAsync(id);
    }
}
