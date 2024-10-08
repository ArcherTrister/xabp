// This file is automatically generated by ABP framework to use MVC Controllers from CSharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client;
using Volo.Abp.Http.Client.ClientProxying;
using Volo.Abp.Http.Modeling;
using X.Abp.IdentityServer.IdentityResource;
using X.Abp.IdentityServer.IdentityResource.Dtos;

// ReSharper disable once CheckNamespace
namespace X.Abp.IdentityServer;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IIdentityResourceAppService), typeof(IdentityResourcesClientProxy))]
public partial class IdentityResourcesClientProxy : ClientProxyBase<IIdentityResourceAppService>, IIdentityResourceAppService
{
    public virtual async Task<PagedResultDto<IdentityResourceWithDetailsDto>> GetListAsync(GetIdentityResourceListInput input)
    {
        return await RequestAsync<PagedResultDto<IdentityResourceWithDetailsDto>>(nameof(GetListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GetIdentityResourceListInput), input }
        });
    }

    public virtual async Task<List<IdentityResourceWithDetailsDto>> GetAllListAsync()
    {
        return await RequestAsync<List<IdentityResourceWithDetailsDto>>(nameof(GetAllListAsync));
    }

    public virtual async Task<IdentityResourceWithDetailsDto> GetAsync(Guid id)
    {
        return await RequestAsync<IdentityResourceWithDetailsDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<IdentityResourceWithDetailsDto> CreateAsync(CreateIdentityResourceDto input)
    {
        return await RequestAsync<IdentityResourceWithDetailsDto>(nameof(CreateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(CreateIdentityResourceDto), input }
        });
    }

    public virtual async Task<IdentityResourceWithDetailsDto> UpdateAsync(Guid id, UpdateIdentityResourceDto input)
    {
        return await RequestAsync<IdentityResourceWithDetailsDto>(nameof(UpdateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(UpdateIdentityResourceDto), input }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await RequestAsync(nameof(DeleteAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task CreateStandardResourcesAsync()
    {
        await RequestAsync(nameof(CreateStandardResourcesAsync));
    }
}
