// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

using X.Abp.Saas.Tenants;

namespace X.Abp.Saas;

public class SaasAppliedDatabaseMigrationsHandler :
    IDistributedEventHandler<AppliedDatabaseMigrationsEto>,
    ITransientDependency
{
    public ILogger<SaasAppliedDatabaseMigrationsHandler> Logger { get; set; }

    protected ITenantRepository TenantRepository { get; }

    protected IDistributedEventBus DistributedEventBus { get; }

    public SaasAppliedDatabaseMigrationsHandler(
      ITenantRepository tenantRepository,
      IDistributedEventBus distributedEventBus)
    {
        Logger = NullLogger<SaasAppliedDatabaseMigrationsHandler>.Instance;
        TenantRepository = tenantRepository;
        DistributedEventBus = distributedEventBus;
    }

    public virtual async Task HandleEventAsync(AppliedDatabaseMigrationsEto eventData)
    {
        Logger.LogDebug($"Handling AppliedDatabaseMigrationsEto event. DatabaseName: {eventData.DatabaseName}");

        if (eventData.TenantId.HasValue)
        {
            Logger.LogDebug("Skipping AppliedDatabaseMigrationsEto event handling since it designed for host only.");
        }
        else
        {
            foreach (Tenant tenant in await TenantRepository.GetListWithSeparateConnectionStringAsync(eventData.DatabaseName))
            {
                Logger.LogDebug($"Publishing ApplyDatabaseMigrationsEto event for tenant: {tenant.Id} {tenant.Name}.");

                await DistributedEventBus.PublishAsync(new ApplyDatabaseMigrationsEto()
                {
                    TenantId = tenant.Id,
                    DatabaseName = eventData.DatabaseName
                });
            }
        }
    }
}
