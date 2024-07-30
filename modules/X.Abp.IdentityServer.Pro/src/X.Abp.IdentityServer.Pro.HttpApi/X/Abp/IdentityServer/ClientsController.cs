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
using Volo.Abp.Auditing;

using X.Abp.IdentityServer.Client;
using X.Abp.IdentityServer.Client.Dtos;

namespace X.Abp.IdentityServer;

[Controller]
[ControllerName("Clients")]
[Route("api/identity-server/clients")]
[Area(AbpIdentityServerProRemoteServiceConsts.ModuleName)]
[RemoteService(Name = AbpIdentityServerProRemoteServiceConsts.RemoteServiceName)]
[DisableAuditing]
public class ClientsController : AbpControllerBase, IClientAppService
{
    protected IClientAppService ClientAppService { get; }

    public ClientsController(IClientAppService clientAppService)
    {
        ClientAppService = clientAppService;
    }

    [HttpGet]
    public virtual async Task<PagedResultDto<ClientWithDetailsDto>> GetListAsync(GetClientListInput input)
    {
        return await ClientAppService.GetListAsync(input);
    }

    [Route("{id}")]
    [HttpGet]
    public virtual async Task<ClientWithDetailsDto> GetAsync(Guid id)
    {
        return await ClientAppService.GetAsync(id);
    }

    [HttpPost]
    public virtual async Task<ClientWithDetailsDto> CreateAsync(CreateClientDto input)
    {
        return await ClientAppService.CreateAsync(input);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual async Task<ClientWithDetailsDto> UpdateAsync(Guid id, UpdateClientDto input)
    {
        return await ClientAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual async Task DeleteAsync(Guid id)
    {
        await ClientAppService.DeleteAsync(id);
    }
}
