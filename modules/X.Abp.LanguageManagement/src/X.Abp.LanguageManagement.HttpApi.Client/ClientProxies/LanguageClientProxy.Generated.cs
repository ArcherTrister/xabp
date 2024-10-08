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
using X.Abp.LanguageManagement;
using X.Abp.LanguageManagement.Dto;

// ReSharper disable once CheckNamespace
namespace X.Abp.LanguageManagement;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(ILanguageAppService), typeof(LanguageClientProxy))]
public partial class LanguageClientProxy : ClientProxyBase<ILanguageAppService>, ILanguageAppService
{
    public virtual async Task<ListResultDto<LanguageDto>> GetAllListAsync()
    {
        return await RequestAsync<ListResultDto<LanguageDto>>(nameof(GetAllListAsync));
    }

    public virtual async Task<PagedResultDto<LanguageDto>> GetListAsync(GetLanguagesTextsInput input)
    {
        return await RequestAsync<PagedResultDto<LanguageDto>>(nameof(GetListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(GetLanguagesTextsInput), input }
        });
    }

    public virtual async Task<LanguageDto> GetAsync(Guid id)
    {
        return await RequestAsync<LanguageDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<LanguageDto> CreateAsync(CreateLanguageDto input)
    {
        return await RequestAsync<LanguageDto>(nameof(CreateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(CreateLanguageDto), input }
        });
    }

    public virtual async Task<LanguageDto> UpdateAsync(Guid id, UpdateLanguageDto input)
    {
        return await RequestAsync<LanguageDto>(nameof(UpdateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(UpdateLanguageDto), input }
        });
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        await RequestAsync(nameof(DeleteAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task SetAsDefaultAsync(Guid id)
    {
        await RequestAsync(nameof(SetAsDefaultAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task<List<LanguageResourceDto>> GetResourcesAsync()
    {
        return await RequestAsync<List<LanguageResourceDto>>(nameof(GetResourcesAsync));
    }

    public virtual async Task<List<CultureInfoDto>> GetCulturelistAsync()
    {
        return await RequestAsync<List<CultureInfoDto>>(nameof(GetCulturelistAsync));
    }

    public virtual async Task<List<String>> GetFlagListAsync()
    {
        return await RequestAsync<List<String>>(nameof(GetFlagListAsync));
    }
}
