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

namespace X.Abp.Identity.Integration
{
  [RemoteService(true, Name = AbpIdentityProRemoteServiceConsts.RemoteServiceName)]
  [Area(AbpIdentityProRemoteServiceConsts.ModuleName)]
  [ControllerName("UserIntegration")]
  [Route("integration-api/identity/users")]
  public class IdentityUserIntegrationController :
    AbpControllerBase,
    IIdentityUserIntegrationService
  {
    protected IIdentityUserIntegrationService UserIntegrationService { get; }

    public IdentityUserIntegrationController(
      IIdentityUserIntegrationService userIntegrationService)
    {
      UserIntegrationService = userIntegrationService;
    }

    [HttpGet]
    [Route("{id}/role-names")]
    public virtual Task<string[]> GetRoleNamesAsync(Guid id) => UserIntegrationService.GetRoleNamesAsync(id);

    [HttpGet]
    [Route("{id}")]
    public virtual Task<UserData> FindByIdAsync(Guid id) => UserIntegrationService.FindByIdAsync(id);

    [Route("by-username/{userName}")]
    [HttpGet]
    public virtual Task<UserData> FindByUserNameAsync(string userName) => UserIntegrationService.FindByUserNameAsync(userName);

    [Route("search")]
    [HttpGet]
    public virtual Task<ListResultDto<UserData>> SearchAsync(
      UserLookupSearchInputDto input)
    {
      return UserIntegrationService.SearchAsync(input);
    }

    [HttpGet]
    [Route("count")]
    public virtual Task<long> GetCountAsync(UserLookupCountInputDto input) => UserIntegrationService.GetCountAsync(input);
  }
}
