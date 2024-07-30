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

namespace X.Abp.Account;

[RemoteService(Name = AbpAccountPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AbpAccountPublicRemoteServiceConsts.ModuleName)]
[Route("api/account/user-delegation")]
[ControllerName("User")]
public class IdentityUserDelegationController : AbpControllerBase, IIdentityUserDelegationAppService
{
    protected IIdentityUserDelegationAppService IdentityUserDelegationAppService { get; }

    public IdentityUserDelegationController(IIdentityUserDelegationAppService identityUserDelegationAppService)
    {
        IdentityUserDelegationAppService = identityUserDelegationAppService;
    }

    [HttpGet]
    [Route("delegated-users")]
    public virtual Task<ListResultDto<UserDelegationDto>> GetDelegatedUsersAsync()
    {
        return IdentityUserDelegationAppService.GetDelegatedUsersAsync();
    }

    [Route("my-delegated-users")]
    [HttpGet]
    public virtual Task<ListResultDto<UserDelegationDto>> GetMyDelegatedUsersAsync()
    {
        return IdentityUserDelegationAppService.GetMyDelegatedUsersAsync();
    }

    [Route("active-delegations")]
    [HttpGet]
    public virtual Task<ListResultDto<UserDelegationDto>> GetActiveDelegationsAsync()
    {
        return IdentityUserDelegationAppService.GetActiveDelegationsAsync();
    }

    [HttpGet]
    [Route("user-lookup")]
    public virtual Task<ListResultDto<UserLookupDto>> GetUserLookupAsync(GetUserLookupInput input)
    {
        return IdentityUserDelegationAppService.GetUserLookupAsync(input);
    }

    [Route("delegate-new-user")]
    [HttpPost]
    public virtual Task DelegateNewUserAsync(DelegateNewUserInput input)
    {
        return IdentityUserDelegationAppService.DelegateNewUserAsync(input);
    }

    [HttpPost]
    [Route("delete-delegation")]
    public virtual Task DeleteDelegationAsync(Guid id)
    {
        return IdentityUserDelegationAppService.DeleteDelegationAsync(id);
    }
}
