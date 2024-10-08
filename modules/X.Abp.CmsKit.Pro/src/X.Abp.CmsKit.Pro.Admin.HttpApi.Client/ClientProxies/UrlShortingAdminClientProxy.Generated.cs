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
using X.Abp.CmsKit.Admin.UrlShorting;

// ReSharper disable once CheckNamespace
namespace X.Abp.CmsKit.Admin.UrlShorting;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IUrlShortingAdminAppService), typeof(UrlShortingAdminClientProxy))]
public partial class UrlShortingAdminClientProxy : ClientProxyBase<IUrlShortingAdminAppService>, IUrlShortingAdminAppService
{
    public virtual async Task<PagedResultDto<ShortenedUrlDto>> GetListAsync(GetShortenedUrlListInput input)
    {
        return await RequestAsync<PagedResultDto<ShortenedUrlDto>>(nameof(GetListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GetShortenedUrlListInput), input }
        });
    }

    public virtual async Task<ShortenedUrlDto> GetAsync(Guid id)
    {
        return await RequestAsync<ShortenedUrlDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<ShortenedUrlDto> CreateAsync(CreateShortenedUrlDto input)
    {
        return await RequestAsync<ShortenedUrlDto>(nameof(CreateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(CreateShortenedUrlDto), input }
        });
    }

    public virtual async Task<ShortenedUrlDto> UpdateAsync(Guid id, UpdateShortenedUrlDto input)
    {
        return await RequestAsync<ShortenedUrlDto>(nameof(UpdateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(UpdateShortenedUrlDto), input }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await RequestAsync(nameof(DeleteAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }
}
