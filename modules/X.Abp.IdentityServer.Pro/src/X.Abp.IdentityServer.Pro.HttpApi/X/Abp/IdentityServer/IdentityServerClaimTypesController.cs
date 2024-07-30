// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Auditing;

using X.Abp.IdentityServer.ClaimType;
using X.Abp.IdentityServer.ClaimType.Dtos;

namespace X.Abp.IdentityServer;

[ControllerName("ClaimTypes")]
[Route("api/identity-server/claim-types")]
[DisableAuditing]
[Area(AbpIdentityServerProRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpIdentityServerProRemoteServiceConsts.RemoteServiceName)]
[Controller]
public class IdentityServerClaimTypesController : AbpControllerBase, IIdentityServerClaimTypeAppService
{
    protected IIdentityServerClaimTypeAppService ClaimTypeAppService { get; }

    public IdentityServerClaimTypesController(IIdentityServerClaimTypeAppService claimTypeAppService)
    {
        ClaimTypeAppService = claimTypeAppService;
    }

    [HttpGet]
    public virtual async Task<List<IdentityClaimTypeDto>> GetListAsync()
    {
        return await ClaimTypeAppService.GetListAsync();
    }
}
