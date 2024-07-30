// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;

using X.Abp.OpenIddict.Applications;
using X.Abp.OpenIddict.Applications.Dtos;

namespace X.Abp.OpenIddict;

[ControllerName("Applications")]
[RemoteService(true, Name = AbpOpenIddictProRemoteServiceConsts.RemoteServiceName)]
[Area(AbpOpenIddictProRemoteServiceConsts.ModuleName)]
[Controller]
[DisableAuditing]
[Route("api/openiddict/applications")]
public class ApplicationController : AbpOpenIddictProController, IApplicationAppService
{
    protected IApplicationAppService ApplicationAppService { get; }

    public ApplicationController(IApplicationAppService applicationAppService)
    {
        ApplicationAppService = applicationAppService;
    }

    [Route("{id}")]
    [HttpGet]
    public virtual Task<ApplicationDto> GetAsync(Guid id)
    {
        return ApplicationAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<ApplicationDto>> GetListAsync(GetApplicationListInput input)
    {
        return ApplicationAppService.GetListAsync(input);
    }

    [HttpPost]
    public virtual Task<ApplicationDto> CreateAsync(CreateApplicationInput input)
    {
        return ApplicationAppService.CreateAsync(input);
    }

    [Route("{id}")]
    [HttpPut]
    public virtual Task<ApplicationDto> UpdateAsync(Guid id, UpdateApplicationInput input)
    {
        return ApplicationAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return ApplicationAppService.DeleteAsync(id);
    }
}
