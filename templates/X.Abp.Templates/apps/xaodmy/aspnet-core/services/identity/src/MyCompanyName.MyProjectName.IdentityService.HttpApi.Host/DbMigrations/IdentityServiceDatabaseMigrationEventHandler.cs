using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using MyCompanyName.MyProjectName.IdentityService.EntityFrameworkCore;
using MyCompanyName.MyProjectName.Shared.Hosting.Microservices.DbMigrations;

using Microsoft.Extensions.Logging;

using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

using X.Abp.Identity;
using X.Abp.Saas.Tenants;

namespace MyCompanyName.MyProjectName.IdentityService.DbMigrations;

public class IdentityServiceDatabaseMigrationEventHandler
    : DatabaseMigrationEventHandlerBase<IdentityServiceDbContext>,
        IDistributedEventHandler<TenantCreatedEto>,
        IDistributedEventHandler<TenantConnectionStringUpdatedEto>,
        IDistributedEventHandler<ApplyDatabaseMigrationsEto>
{
    private readonly IdentityServiceDataSeeder _identityServiceDataSeeder;

    public IdentityServiceDatabaseMigrationEventHandler(
        ILoggerFactory loggerFactory,
        ICurrentTenant currentTenant,
        IUnitOfWorkManager unitOfWorkManager,
        ITenantStore tenantStore,
        ITenantRepository tenantRepository,
        IDistributedEventBus distributedEventBus,
        IdentityServiceDataSeeder identityServiceDataSeeder)
        : base(
        loggerFactory,
        currentTenant,
        unitOfWorkManager,
        tenantStore,
        tenantRepository,
        distributedEventBus,
        IdentityServiceDbProperties.ConnectionStringName)
    {
        _identityServiceDataSeeder = identityServiceDataSeeder;
    }

    public async Task HandleEventAsync(ApplyDatabaseMigrationsEto eventData)
    {
        if (eventData.DatabaseName != DatabaseName)
        {
            return;
        }

        try
        {
            var schemaMigrated = await MigrateDatabaseSchemaAsync(eventData.TenantId);
            await _identityServiceDataSeeder.SeedAsync(
                tenantId: eventData.TenantId,
                adminEmail: IdentityDataSeedContributor.AdminEmailDefaultValue,
                adminPassword: IdentityDataSeedContributor.AdminPasswordDefaultValue);

            if (eventData.TenantId == null && schemaMigrated)
            {
                /* Migrate tenant databases after host migration */
                await QueueTenantMigrationsAsync();
            }
        }
        catch (Exception ex)
        {
            await HandleErrorOnApplyDatabaseMigrationAsync(eventData, ex);
        }
    }

    public async Task HandleEventAsync(TenantCreatedEto eventData)
    {
        try
        {
            await MigrateDatabaseSchemaAsync(eventData.Id);
            await _identityServiceDataSeeder.SeedAsync(
                tenantId: eventData.Id,
                adminEmail: eventData.Properties.GetOrDefault(IdentityDataSeedContributor.AdminEmailPropertyName) ?? IdentityDataSeedContributor.AdminEmailDefaultValue,
                adminPassword: eventData.Properties.GetOrDefault(IdentityDataSeedContributor.AdminPasswordPropertyName) ?? IdentityDataSeedContributor.AdminPasswordDefaultValue);
        }
        catch (Exception ex)
        {
            await HandleErrorTenantCreatedAsync(eventData, ex);
        }
    }

    public async Task HandleEventAsync(TenantConnectionStringUpdatedEto eventData)
    {
        if ((eventData.ConnectionStringName != DatabaseName && eventData.ConnectionStringName != ConnectionStrings.DefaultConnectionStringName) ||
            eventData.NewValue.IsNullOrWhiteSpace())
        {
            return;
        }

        try
        {
            await MigrateDatabaseSchemaAsync(eventData.Id);
            await _identityServiceDataSeeder.SeedAsync(
                tenantId: eventData.Id,
                adminEmail: IdentityDataSeedContributor.AdminEmailDefaultValue,
                adminPassword: IdentityDataSeedContributor.AdminPasswordDefaultValue);

            /* You may want to move your data from the old database to the new database!
             * It is up to you. If you don't make it, new database will be empty
             * (and tenant's admin password is reset to IdentityDataSeedContributor.AdminPasswordDefaultValue). */
        }
        catch (Exception ex)
        {
            await HandleErrorTenantConnectionStringUpdatedAsync(eventData, ex);
        }
    }
}
