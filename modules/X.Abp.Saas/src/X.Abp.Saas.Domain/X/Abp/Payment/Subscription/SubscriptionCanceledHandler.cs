// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Distributed;

using X.Abp.Saas;
using X.Abp.Saas.Tenants;

namespace X.Abp.Payment.Subscription;

public class SubscriptionCanceledHandler :
IDistributedEventHandler<SubscriptionCanceledEto>,
ITransientDependency,
IEventHandler
{
    protected ITenantRepository TenantRepository { get; }

    protected ILogger<SubscriptionCanceledHandler> Logger { get; }

    public SubscriptionCanceledHandler(
      ITenantRepository tenantRepository,
      ILogger<SubscriptionCanceledHandler> logger)
    {
        TenantRepository = tenantRepository;
        Logger = logger;
    }

    public async Task HandleEventAsync(SubscriptionCanceledEto eventData)
    {
        var tenant = await TenantRepository.FindAsync(Guid.Parse(eventData.ExtraProperties[TenantConsts.TenantIdParameterName]?.ToString()), false);

        if (tenant == null)
        {
            Logger.LogWarning(TenantConsts.TenantIdParameterName + eventData.PaymentRequestId.ToString());
        }
        else
        {
            tenant.EditionEndDateUtc = eventData.PeriodEndDate;
            await TenantRepository.UpdateAsync(tenant, false);
        }
    }
}
