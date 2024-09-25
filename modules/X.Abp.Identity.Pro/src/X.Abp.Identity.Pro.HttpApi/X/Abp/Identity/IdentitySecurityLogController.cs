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

namespace X.Abp.Identity;

[RemoteService(Name = AbpIdentityProRemoteServiceConsts.RemoteServiceName)]
[Area(AbpIdentityProRemoteServiceConsts.ModuleName)]
[ControllerName("SecurityLog")]
[Route("api/identity/security-logs")]
public class IdentitySecurityLogController : AbpControllerBase, IIdentitySecurityLogAppService
{
  protected IIdentitySecurityLogAppService IdentitySecurityLogAppService { get; }

  public IdentitySecurityLogController(IIdentitySecurityLogAppService identitySecurityLogAppService)
  {
    IdentitySecurityLogAppService = identitySecurityLogAppService;
  }

  [HttpGet]
  public virtual Task<PagedResultDto<IdentitySecurityLogDto>> GetListAsync([FromQuery] GetIdentitySecurityLogListInput input)
  {
    return IdentitySecurityLogAppService.GetListAsync(input);
  }

  [HttpGet]
  [Route("{id}")]
  public virtual Task<IdentitySecurityLogDto> GetAsync(Guid id)
  {
    return IdentitySecurityLogAppService.GetAsync(id);
  }

  [HttpGet]
  [Route("my")]
  public virtual Task<PagedResultDto<IdentitySecurityLogDto>> GetMyListAsync([FromQuery] GetIdentitySecurityLogListInput input)
  {
    return IdentitySecurityLogAppService.GetMyListAsync(input);
  }

  [HttpGet]
  [Route("my/{id}")]
  public virtual Task<IdentitySecurityLogDto> GetMyAsync(Guid id)
  {
    return IdentitySecurityLogAppService.GetMyAsync(id);
  }
}
