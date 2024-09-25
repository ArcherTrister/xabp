// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.GlobalFeatures;
using Volo.CmsKit.Admin;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Admin.Polls;

[Route("api/cms-kit-admin/poll")]
[Authorize(AbpCmsKitProAdminPermissions.Polls.Default)]
[Area(AbpCmsKitProAdminRemoteServiceConsts.ModuleName)]
[RequiresGlobalFeature(typeof(PollsFeature))]
[RemoteService(true, Name = CmsKitAdminRemoteServiceConsts.RemoteServiceName)]
public class PollAdminController : CmsKitProAdminController, IPollAdminAppService
{
  protected IPollAdminAppService PollAdminAppService { get; }

  public PollAdminController(IPollAdminAppService pollAdminAppService)
  {
    PollAdminAppService = pollAdminAppService;
  }

  [HttpGet]
  public virtual Task<PagedResultDto<PollDto>> GetListAsync(GetPollListInput input)
  {
    return PollAdminAppService.GetListAsync(input);
  }

  [HttpGet]
  [Route("{id}")]
  public virtual Task<PollWithDetailsDto> GetAsync(Guid id)
  {
    return PollAdminAppService.GetAsync(id);
  }

  [Authorize(AbpCmsKitProAdminPermissions.Polls.Create)]
  [HttpPost]
  public virtual Task<PollWithDetailsDto> CreateAsync(CreatePollDto input)
  {
    return PollAdminAppService.CreateAsync(input);
  }

  [HttpPut]
  [Authorize(AbpCmsKitProAdminPermissions.Polls.Update)]
  [Route("{id}")]
  public virtual Task<PollWithDetailsDto> UpdateAsync(Guid id, UpdatePollDto input)
  {
    return PollAdminAppService.UpdateAsync(id, input);
  }

  [Authorize(AbpCmsKitProAdminPermissions.Polls.Delete)]
  [Route("{id}")]
  [HttpDelete]
  public virtual Task DeleteAsync(Guid id)
  {
    return PollAdminAppService.DeleteAsync(id);
  }

  [HttpGet]
  [Route("widgets")]
  public virtual Task<ListResultDto<PollWidgetDto>> GetWidgetsAsync()
  {
    return PollAdminAppService.GetWidgetsAsync();
  }

  [HttpGet]
  [Route("result")]
  public virtual Task<GetResultDto> GetResultAsync(Guid id)
  {
    return PollAdminAppService.GetResultAsync(id);
  }
}
