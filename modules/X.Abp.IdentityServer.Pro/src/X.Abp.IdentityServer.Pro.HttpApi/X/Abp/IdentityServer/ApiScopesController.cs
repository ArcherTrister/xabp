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

using X.Abp.IdentityServer.ApiScope;
using X.Abp.IdentityServer.ApiScope.Dtos;

namespace X.Abp.IdentityServer;

[ControllerName("ApiScopes")]
[DisableAuditing]
[Route("api/identity-server/api-scopes")]
[RemoteService(Name = AbpIdentityServerProRemoteServiceConsts.RemoteServiceName)]
[Area(AbpIdentityServerProRemoteServiceConsts.ModuleName)]
[Controller]
public class ApiScopeController : AbpController, IApiScopeAppService
{
    protected IApiScopeAppService ApiScopeAppService { get; }

    public ApiScopeController(
        IApiScopeAppService apiScopeAppService)
    {
        ApiScopeAppService = apiScopeAppService;
    }

    [HttpPost]
    public virtual async Task<ApiScopeWithDetailsDto> CreateAsync(CreateApiScopeDto input)
    {
        return await ApiScopeAppService.CreateAsync(input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual async Task DeleteAsync(Guid id)
    {
        await ApiScopeAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual async Task<ApiScopeWithDetailsDto> GetAsync(Guid id)
    {
        return await ApiScopeAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual async Task<PagedResultDto<ApiScopeWithDetailsDto>> GetListAsync(GetApiScopeListInput input)
    {
        return await ApiScopeAppService.GetListAsync(input);
    }

    [Route("all")]
    [HttpGet]
    public virtual async Task<List<ApiScopeWithDetailsDto>> GetAllListAsync()
    {
        return await ApiScopeAppService.GetAllListAsync();
    }

    [HttpPut]
    [Route("{id}")]
    public virtual async Task<ApiScopeWithDetailsDto> UpdateAsync(Guid id, UpdateApiScopeDto input)
    {
        return await ApiScopeAppService.UpdateAsync(id, input);
    }
}
