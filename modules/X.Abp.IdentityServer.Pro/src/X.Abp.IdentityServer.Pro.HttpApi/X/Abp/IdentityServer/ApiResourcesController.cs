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

using X.Abp.IdentityServer.ApiResource;
using X.Abp.IdentityServer.ApiResource.Dtos;

namespace X.Abp.IdentityServer;

[Area(AbpIdentityServerProRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpIdentityServerProRemoteServiceConsts.RemoteServiceName)]
[Route("api/identity-server/api-resources")]
[DisableAuditing]
[Controller]
[ControllerName("ApiResources")]
public class ApiResourcesController : AbpControllerBase, IApiResourceAppService
{
    protected IApiResourceAppService ApiResourceAppService { get; }

    public ApiResourcesController(IApiResourceAppService apiResourceAppService)
    {
        ApiResourceAppService = apiResourceAppService;
    }

    [HttpGet]
    public virtual async Task<PagedResultDto<ApiResourceWithDetailsDto>> GetListAsync(GetApiResourceListInput input)
    {
        return await ApiResourceAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("all")]
    public virtual async Task<List<ApiResourceWithDetailsDto>> GetAllListAsync()
    {
        return await ApiResourceAppService.GetAllListAsync();
    }

    [Route("{id}")]
    [HttpGet]
    public virtual async Task<ApiResourceWithDetailsDto> GetAsync(Guid id)
    {
        return await ApiResourceAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual async Task<ApiResourceWithDetailsDto> CreateAsync(CreateApiResourceDto input)
    {
        return await ApiResourceAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual async Task<ApiResourceWithDetailsDto> UpdateAsync(Guid id, UpdateApiResourceDto input)
    {
        return await ApiResourceAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual async Task DeleteAsync(Guid id)
    {
        await ApiResourceAppService.DeleteAsync(id);
    }
}
