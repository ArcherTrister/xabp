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
[ControllerName("ClaimType")]
[Route("api/identity/claim-types")]
public class IdentityClaimTypeController : AbpControllerBase, IIdentityClaimTypeAppService
{
    protected IIdentityClaimTypeAppService ClaimTypeAppService { get; }

    public IdentityClaimTypeController(IIdentityClaimTypeAppService claimTypeAppService)
    {
        ClaimTypeAppService = claimTypeAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<ClaimTypeDto>> GetListAsync(GetIdentityClaimTypesInput input)
    {
        return ClaimTypeAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<ClaimTypeDto> GetAsync(Guid id)
    {
        return ClaimTypeAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual Task<ClaimTypeDto> CreateAsync(CreateClaimTypeDto input)
    {
        return ClaimTypeAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<ClaimTypeDto> UpdateAsync(Guid id, UpdateClaimTypeDto input)
    {
        return ClaimTypeAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return ClaimTypeAppService.DeleteAsync(id);
    }
}
