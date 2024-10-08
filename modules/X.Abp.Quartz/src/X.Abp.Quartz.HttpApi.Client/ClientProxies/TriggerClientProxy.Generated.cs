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
using X.Abp.Quartz.Dtos;
using X.Abp.Quartz.Triggers;
using X.Abp.Quartz.Triggers.Dtos;

// ReSharper disable once CheckNamespace
namespace X.Abp.Quartz;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(ITriggerAppService), typeof(TriggerClientProxy))]
public partial class TriggerClientProxy : ClientProxyBase<ITriggerAppService>, ITriggerAppService
{
    public virtual async Task<IReadOnlyList<JobOrTriggerKeyDto>> GetListAsync(string schedulerName, GroupMatcherDto groupMatcher)
    {
        return await RequestAsync<IReadOnlyList<JobOrTriggerKeyDto>>(nameof(GetListAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), schedulerName },
            { typeof(GroupMatcherDto), groupMatcher }
        });
    }

    public virtual async Task<TriggerDetailDto> GetAsync(string schedulerName, string triggerGroup, string triggerName)
    {
        return await RequestAsync<TriggerDetailDto>(nameof(GetAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), schedulerName },
            { typeof(string), triggerGroup },
            { typeof(string), triggerName }
        });
    }

    public virtual async Task PauseAsync(string schedulerName, string triggerGroup, string triggerName)
    {
        await RequestAsync(nameof(PauseAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), schedulerName },
            { typeof(string), triggerGroup },
            { typeof(string), triggerName }
        });
    }

    public virtual async Task BatchPauseAsync(string schedulerName, GroupMatcherDto groupMatcher)
    {
        await RequestAsync(nameof(BatchPauseAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), schedulerName },
            { typeof(GroupMatcherDto), groupMatcher }
        });
    }

    public virtual async Task ResumeAsync(string schedulerName, string triggerGroup, string triggerName)
    {
        await RequestAsync(nameof(ResumeAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), schedulerName },
            { typeof(string), triggerGroup },
            { typeof(string), triggerName }
        });
    }

    public virtual async Task BatchResumeAsync(string schedulerName, GroupMatcherDto groupMatcher)
    {
        await RequestAsync(nameof(BatchResumeAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), schedulerName },
            { typeof(GroupMatcherDto), groupMatcher }
        });
    }
}
