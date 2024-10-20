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
using X.Abp.CmsKit.Public.Polls;

// ReSharper disable once CheckNamespace
namespace X.Abp.CmsKit.Public.Polls;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(IPollPublicAppService), typeof(PollPublicClientProxy))]
public partial class PollPublicClientProxy : ClientProxyBase<IPollPublicAppService>, IPollPublicAppService
{
    public virtual async Task<bool> IsWidgetNameAvailableAsync(string widgetName)
    {
        return await RequestAsync<bool>(nameof(IsWidgetNameAvailableAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), widgetName }
        });
    }

    public virtual async Task<PollWithDetailsDto> FindByAvailableWidgetAsync(string widgetName)
    {
        return await RequestAsync<PollWithDetailsDto>(nameof(FindByAvailableWidgetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), widgetName }
        });
    }

    public virtual async Task<PollWithDetailsDto> FindByCodeAsync(string code)
    {
        return await RequestAsync<PollWithDetailsDto>(nameof(FindByCodeAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), code }
        });
    }

    public virtual async Task<GetResultDto> GetResultAsync(Guid id)
    {
        return await RequestAsync<GetResultDto>(nameof(GetResultAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id }
        });
    }

    public virtual async Task SubmitVoteAsync(Guid id, SubmitPollInput input)
    {
        await RequestAsync(nameof(SubmitVoteAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), id },
            { typeof(SubmitPollInput), input }
        });
    }
}
