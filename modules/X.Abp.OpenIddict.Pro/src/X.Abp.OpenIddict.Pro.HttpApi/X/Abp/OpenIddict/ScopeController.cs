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
using Volo.Abp.Auditing;

using X.Abp.OpenIddict.Scopes;
using X.Abp.OpenIddict.Scopes.Dtos;

namespace X.Abp.OpenIddict;

[Route("api/openiddict/scopes")]
[ControllerName("Scopes")]
[DisableAuditing]
[Controller]
[RemoteService(true, Name = AbpOpenIddictProRemoteServiceConsts.RemoteServiceName)]
[Area(AbpOpenIddictProRemoteServiceConsts.ModuleName)]
public class ScopeController : AbpOpenIddictProController, IScopeAppService
{
    protected IScopeAppService ScopeAppService { get; }

    public ScopeController(IScopeAppService scopeAppService)
    {
        ScopeAppService = scopeAppService;
    }

    [Route("{id}")]
    [HttpGet]
    public virtual Task<ScopeDto> GetAsync(Guid id)
    {
        return ScopeAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<ScopeDto>> GetListAsync(GetScopeListInput input)
    {
        return ScopeAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<ScopeDto> CreateAsync(CreateScopeInput input)
    {
        return ScopeAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<ScopeDto> UpdateAsync(Guid id, UpdateScopeInput input)
    {
        return ScopeAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return ScopeAppService.DeleteAsync(id);
    }

    [HttpGet]
    [Route("all")]
    public virtual Task<List<ScopeDto>> GetAllScopesAsync()
    {
        return ScopeAppService.GetAllScopesAsync();
    }
}
