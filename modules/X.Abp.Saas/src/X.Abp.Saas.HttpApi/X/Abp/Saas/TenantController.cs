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

using X.Abp.Saas.Dtos;

namespace X.Abp.Saas;

[Controller]
[Route("/api/saas/tenants")]
[ControllerName("Tenant")]
[RemoteService(true, Name = AbpSaasRemoteServiceConsts.RemoteServiceName)]
[Area(AbpSaasRemoteServiceConsts.ModuleName)]
public class TenantController : AbpControllerBase, ITenantAppService
{
    protected ITenantAppService Service { get; }

    public TenantController(ITenantAppService service)
    {
        Service = service;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<SaasTenantDto> GetAsync(Guid id)
    {
        return Service.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<SaasTenantDto>> GetListAsync(GetTenantsInput input)
    {
        return Service.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<SaasTenantDto> CreateAsync(SaasTenantCreateDto input)
    {
        ValidateModel();
        return Service.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<SaasTenantDto> UpdateAsync(Guid id, SaasTenantUpdateDto input)
    {
        return Service.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return Service.DeleteAsync(id);
    }

    [Route("databases")]
    [HttpGet]
    public virtual Task<SaasTenantDatabasesDto> GetDatabasesAsync()
    {
        return Service.GetDatabasesAsync();
    }

    [HttpGet]
    [Route("{id}/connection-strings")]
    public virtual Task<SaasTenantConnectionStringsDto> GetConnectionStringsAsync(Guid id)
    {
        return Service.GetConnectionStringsAsync(id);
    }

    [Route("{id}/connection-strings")]
    [HttpPut]
    public virtual Task UpdateConnectionStringsAsync(Guid id, SaasTenantConnectionStringsDto input)
    {
        return Service.UpdateConnectionStringsAsync(id, input);
    }

    [HttpPost]
    [Route("{id}/apply-database-migrations")]
    public virtual Task ApplyDatabaseMigrationsAsync(Guid id)
    {
        return Service.ApplyDatabaseMigrationsAsync(id);
    }

    [HttpGet]
    [Route("lookup/editions")]
    public virtual Task<List<EditionLookupDto>> GetEditionLookupAsync()
    {
        return Service.GetEditionLookupAsync();
    }

    [HttpPost]
    [Route("{id}/set-password")]
    public virtual Task SetPasswordAsync(Guid id, SaasTenantSetPasswordDto input)
    {
        return Service.SetPasswordAsync(id, input);
    }
}
