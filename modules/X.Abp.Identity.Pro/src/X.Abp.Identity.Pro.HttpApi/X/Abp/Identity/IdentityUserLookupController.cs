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
using Volo.Abp.Users;

namespace X.Abp.Identity;

[RemoteService(Name = AbpIdentityProRemoteServiceConsts.RemoteServiceName)]
[Area(AbpIdentityProRemoteServiceConsts.ModuleName)]
[ControllerName("UserLookup")]
[Route("api/identity/users/lookup")]
public class IdentityUserLookupController : AbpControllerBase, IIdentityUserLookupAppService
{
    protected IIdentityUserLookupAppService LookupAppService { get; }

    public IdentityUserLookupController(IIdentityUserLookupAppService lookupAppService)
    {
        LookupAppService = lookupAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<UserData> FindByIdAsync(Guid id)
    {
        return LookupAppService.FindByIdAsync(id);
    }

    [HttpGet]
    [Route("by-username/{userName}")]
    public virtual Task<UserData> FindByUserNameAsync(string userName)
    {
        return LookupAppService.FindByUserNameAsync(userName);
    }

    [HttpGet]
    [Route("search")]
    public Task<ListResultDto<UserData>> SearchAsync(UserLookupSearchInputDto input)
    {
        return LookupAppService.SearchAsync(input);
    }

    [HttpGet]
    [Route("count")]
    public Task<long> GetCountAsync(UserLookupCountInputDto input)
    {
        return LookupAppService.GetCountAsync(input);
    }
}
