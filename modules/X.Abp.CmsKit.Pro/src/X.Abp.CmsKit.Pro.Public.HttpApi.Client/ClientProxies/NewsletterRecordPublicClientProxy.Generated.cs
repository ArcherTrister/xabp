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
using X.Abp.CmsKit.Public.Newsletters;

// ReSharper disable once CheckNamespace
namespace X.Abp.CmsKit.Public.Newsletters;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(INewsletterRecordPublicAppService), typeof(NewsletterRecordPublicClientProxy))]
public partial class NewsletterRecordPublicClientProxy : ClientProxyBase<INewsletterRecordPublicAppService>, INewsletterRecordPublicAppService
{
    public virtual async Task CreateAsync(CreateNewsletterRecordInput input)
    {
        await RequestAsync(nameof(CreateAsync), new ClientProxyRequestTypeValue
        {
            { typeof(CreateNewsletterRecordInput), input }
        });
    }

    public virtual async Task<List<NewsletterPreferenceDetailsDto>> GetNewsletterPreferencesAsync(string emailAddress)
    {
        return await RequestAsync<List<NewsletterPreferenceDetailsDto>>(nameof(GetNewsletterPreferencesAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), emailAddress }
        });
    }

    public virtual async Task UpdatePreferencesAsync(UpdatePreferenceRequestInput input)
    {
        await RequestAsync(nameof(UpdatePreferencesAsync), new ClientProxyRequestTypeValue
        {
            { typeof(UpdatePreferenceRequestInput), input }
        });
    }

    public virtual async Task<NewsletterEmailOptionsDto> GetOptionByPreferenceAsync(string preference)
    {
        return await RequestAsync<NewsletterEmailOptionsDto>(nameof(GetOptionByPreferenceAsync), new ClientProxyRequestTypeValue
        {
            { typeof(string), preference }
        });
    }
}
