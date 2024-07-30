// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Payment.Plans;
using X.Abp.Saas.Dtos;
using X.Abp.Saas.Permissions;

namespace X.Abp.Saas;

[ControllerName("Edition")]
[Route("/api/saas/editions")]
[RemoteService(true, Name = AbpSaasRemoteServiceConsts.RemoteServiceName)]
[Controller]
[Area(AbpSaasRemoteServiceConsts.ModuleName)]
[Authorize(AbpSaasPermissions.Editions.Default)]
public class EditionController : AbpControllerBase, IEditionAppService
{
    protected IEditionAppService Service { get; }

    public EditionController(IEditionAppService service)
    {
        Service = service;
    }

    [Route("{id}")]
    [HttpGet]
    public virtual Task<EditionDto> GetAsync(Guid id)
    {
        return Service.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<EditionDto>> GetListAsync(GetEditionsInput input)
    {
        return Service.GetListAsync(input);
    }

    [HttpPost]
    [Authorize(AbpSaasPermissions.Editions.Create)]
    public virtual Task<EditionDto> CreateAsync(EditionCreateDto input)
    {
        return Service.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(AbpSaasPermissions.Editions.Update)]
    public virtual Task<EditionDto> UpdateAsync(Guid id, EditionUpdateDto input)
    {
        return Service.UpdateAsync(id, input);
    }

    [Route("{id}")]
    [HttpDelete]
    [Authorize(AbpSaasPermissions.Editions.Delete)]
    public virtual Task DeleteAsync(Guid id)
    {
        return Service.DeleteAsync(id);
    }

    [HttpGet]
    [Route("statistics/usage-statistic")]
    public virtual Task<GetEditionUsageStatisticsResultDto> GetUsageStatisticsAsync()
    {
        return Service.GetUsageStatisticsAsync();
    }

    [Route("plan-lookup")]
    [HttpGet]
    public Task<List<PlanDto>> GetPlanLookupAsync()
    {
        return Service.GetPlanLookupAsync();
    }
}
