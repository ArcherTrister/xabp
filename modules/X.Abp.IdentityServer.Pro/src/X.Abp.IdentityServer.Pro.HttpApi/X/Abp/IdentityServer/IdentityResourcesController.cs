// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;

using X.Abp.IdentityServer.IdentityResource;
using X.Abp.IdentityServer.IdentityResource.Dtos;

namespace X.Abp.IdentityServer;

[ControllerName("IdentityResources")]
[Route("api/identity-server/identity-resources")]
[DisableAuditing]
[Area(AbpIdentityServerProRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpIdentityServerProRemoteServiceConsts.RemoteServiceName)]
[Controller]
public class IdentityResourcesController : AbpControllerBase, IIdentityResourceAppService
{
  protected IIdentityResourceAppService IdentityResourceAppService { get; }

  public IdentityResourcesController(IIdentityResourceAppService identityResourceAppService)
  {
    IdentityResourceAppService = identityResourceAppService;
  }

  [HttpGet]
  public virtual async Task<PagedResultDto<IdentityResourceWithDetailsDto>> GetListAsync(GetIdentityResourceListInput input)
  {
    return await IdentityResourceAppService.GetListAsync(input);
  }

  [Route("all")]
  [HttpGet]
  public virtual async Task<List<IdentityResourceWithDetailsDto>> GetAllListAsync()
  {
    return await IdentityResourceAppService.GetAllListAsync();
  }

  [HttpGet]
  [Route("{id}")]
  public virtual async Task<IdentityResourceWithDetailsDto> GetAsync(Guid id)
  {
    return await IdentityResourceAppService.GetAsync(id);
  }

  [HttpPost]
  public virtual async Task<IdentityResourceWithDetailsDto> CreateAsync(CreateIdentityResourceDto input)
  {
    return await IdentityResourceAppService.CreateAsync(input);
  }

  [Route("{id}")]
  [HttpPut]
  public virtual async Task<IdentityResourceWithDetailsDto> UpdateAsync(Guid id, UpdateIdentityResourceDto input)
  {
    return await IdentityResourceAppService.UpdateAsync(id, input);
  }

  [HttpDelete]
  [Route("{id}")]
  public virtual async Task DeleteAsync(Guid id)
  {
    await IdentityResourceAppService.DeleteAsync(id);
  }

  [HttpPost]
  [Route("create-standard-resources")]
  public virtual async Task CreateStandardResourcesAsync()
  {
    await IdentityResourceAppService.CreateStandardResourcesAsync();
  }
}
