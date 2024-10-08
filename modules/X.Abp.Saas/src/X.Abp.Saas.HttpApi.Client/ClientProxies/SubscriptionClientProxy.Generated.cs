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
using X.Abp.Payment.Requests;
using X.Abp.Saas.Subscription;

// ReSharper disable once CheckNamespace
namespace X.Abp.Saas;

[Dependency(ReplaceServices = true)]
[ExposeServices(typeof(ISubscriptionAppService), typeof(SubscriptionClientProxy))]
public partial class SubscriptionClientProxy : ClientProxyBase<ISubscriptionAppService>, ISubscriptionAppService
{
    public virtual async Task<PaymentRequestWithDetailsDto> CreateSubscriptionAsync(Guid editionId, Guid tenantId)
    {
        return await RequestAsync<PaymentRequestWithDetailsDto>(nameof(CreateSubscriptionAsync), new ClientProxyRequestTypeValue
        {
            { typeof(Guid), editionId },
            { typeof(Guid), tenantId }
        });
    }
}
