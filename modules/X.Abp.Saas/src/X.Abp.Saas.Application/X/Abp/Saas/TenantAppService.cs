// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectExtending;
using Volo.Abp.Uow;
using Volo.Abp.Users;

using X.Abp.Saas.Dtos;
using X.Abp.Saas.Editions;
using X.Abp.Saas.Permissions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas;

[Authorize(AbpSaasPermissions.Tenants.Default)]
public class TenantAppService : SaasAppServiceBase, ITenantAppService
{
    protected IEditionRepository EditionRepository { get; }

    protected IDataSeeder DataSeeder { get; }

    protected IDistributedEventBus DistributedEventBus { get; }

    protected ITenantRepository TenantRepository { get; }

    protected ITenantManager TenantManager { get; }

    protected AbpDbConnectionOptions DbConnectionOptions { get; }

    public TenantAppService(ITenantRepository tenantRepository, IEditionRepository editionRepository, ITenantManager tenantManager, IDataSeeder dataSeeder, IDistributedEventBus distributedEventBus, IOptions<AbpDbConnectionOptions> dbConnectionOptions)
    {
        EditionRepository = editionRepository;
        DataSeeder = dataSeeder;
        DistributedEventBus = distributedEventBus;
        DbConnectionOptions = dbConnectionOptions.Value;
        TenantRepository = tenantRepository;
        TenantManager = tenantManager;
    }

    public virtual async Task<SaasTenantDto> GetAsync(Guid id)
    {
        var tenant = ObjectMapper.Map<Tenant, SaasTenantDto>(await TenantRepository.GetAsync(id));
        if (tenant.EditionEndDateUtc <= DateTime.UtcNow)
        {
            tenant.EditionId = null;
        }

        if (tenant.EditionId.HasValue)
        {
            tenant.EditionName = (await EditionRepository.GetAsync(tenant.EditionId.Value)).DisplayName;
        }

        return tenant;
    }

    public virtual async Task<PagedResultDto<SaasTenantDto>> GetListAsync(GetTenantsInput input)
    {
        var count = await TenantRepository.GetCountAsync(input.Filter, input.EditionId, input.ExpirationDateMin, input.ExpirationDateMax, input.ActivationState);
        var list = await TenantRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter, true, input.EditionId, input.ExpirationDateMin, input.ExpirationDateMax, input.ActivationState);
        var tenantDtos = ObjectMapper.Map<List<Tenant>, List<SaasTenantDto>>(list);
        var editions = !input.GetEditionNames ? null : await EditionRepository.GetListAsync();

        foreach (var tenant in tenantDtos)
        {
            if (tenant.EditionEndDateUtc <= DateTime.UtcNow)
            {
                tenant.EditionId = null;
            }

            if (input.GetEditionNames && tenant.EditionId.HasValue)
            {
                var edition = editions?.FirstOrDefault(p => p.Id == tenant.EditionId);
                tenant.EditionName = edition?.DisplayName;
            }
        }

        return new PagedResultDto<SaasTenantDto>(count, tenantDtos);
    }

    [Authorize(AbpSaasPermissions.Tenants.Create)]
    public virtual async Task<SaasTenantDto> CreateAsync(SaasTenantCreateDto input)
    {
        Tenant tenant = null;
        using (var unitOfWork = UnitOfWorkManager.Begin(true))
        {
            tenant = await CreateTenantAsync(input);
            await unitOfWork.CompleteAsync();
        }

        var tenantCreatedEto = new TenantCreatedEto()
        {
            Id = tenant.Id,
            Name = tenant.Name
        };
        tenantCreatedEto.Properties.Add("AdminEmail", input.AdminEmailAddress);
        tenantCreatedEto.Properties.Add("AdminPassword", input.AdminPassword);
        await DistributedEventBus.PublishAsync(tenantCreatedEto);

        return ObjectMapper.Map<Tenant, SaasTenantDto>(tenant);
    }

    private async Task<Tenant> CreateTenantAsync(SaasTenantCreateDto input)
    {
        var tenant = await TenantManager.CreateAsync(input.Name, input.EditionId);
        input.ConnectionStrings = await NormalizedConnectionStringsAsync(input.ConnectionStrings);
        if (!input.ConnectionStrings.Default.IsNullOrWhiteSpace())
        {
            tenant.SetDefaultConnectionString(input.ConnectionStrings.Default);
        }

        if (!input.ConnectionStrings.Databases.IsNullOrEmpty())
        {
            foreach (var database in input.ConnectionStrings.Databases)
            {
                tenant.SetConnectionString(database.DatabaseName, database.ConnectionString);
            }
        }

        input.MapExtraPropertiesTo(tenant);
        tenant.SetActivationState(input.ActivationState);
        if (tenant.ActivationState == TenantActivationState.ActiveWithLimitedTime)
        {
            tenant.SetActivationEndDate(input.ActivationEndDate);
        }

        return await TenantRepository.InsertAsync(tenant);
    }

    [Authorize(AbpSaasPermissions.Tenants.Update)]
    public virtual async Task<SaasTenantDto> UpdateAsync(Guid id, SaasTenantUpdateDto input)
    {
        var tenant = await TenantRepository.GetAsync(id);
        tenant.EditionId = input.EditionId;
        await TenantManager.ChangeNameAsync(tenant, input.Name);
        tenant.SetActivationState(input.ActivationState);
        tenant.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);
        input.MapExtraPropertiesTo(tenant);
        if (tenant.ActivationState == TenantActivationState.ActiveWithLimitedTime)
        {
            tenant.SetActivationEndDate(input.ActivationEndDate);
        }

        var source = await TenantRepository.UpdateAsync(tenant, false);
        return ObjectMapper.Map<Tenant, SaasTenantDto>(source);
    }

    [Authorize(AbpSaasPermissions.Tenants.Delete)]
    public virtual async Task DeleteAsync(Guid id)
    {
        var tenant = await TenantRepository.FindAsync(id);
        if (tenant != null)
        {
            await TenantRepository.DeleteAsync(tenant);
        }
    }

    [Authorize(AbpSaasPermissions.Tenants.ManageConnectionStrings)]
    public virtual Task<SaasTenantDatabasesDto> GetDatabasesAsync()
    {
        var saasTenantDatabasesDto = new SaasTenantDatabasesDto
        {
            Databases = (from x in DbConnectionOptions.Databases
                         where x.Value.IsUsedByTenants
                         select x.Key).ToList()
        };
        return Task.FromResult(saasTenantDatabasesDto);
    }

    [Authorize(AbpSaasPermissions.Tenants.ManageConnectionStrings)]
    public virtual async Task<SaasTenantConnectionStringsDto> GetConnectionStringsAsync(Guid id)
    {
        var tenant = await TenantRepository.GetAsync(id);
        var saasTenantConnectionStringsDto = new SaasTenantConnectionStringsDto
        {
            Default = tenant.FindDefaultConnectionString(),
            Databases = new List<SaasTenantDatabaseConnectionStringsDto>()
        };
        foreach (var item in DbConnectionOptions.Databases.Where(x => x.Value.IsUsedByTenants))
        {
            saasTenantConnectionStringsDto.Databases.Add(new SaasTenantDatabaseConnectionStringsDto
            {
                DatabaseName = item.Key,
                ConnectionString = tenant.FindConnectionString(item.Key)
            });
        }

        return saasTenantConnectionStringsDto;
    }

    [Authorize(AbpSaasPermissions.Tenants.ManageConnectionStrings)]
    public virtual async Task UpdateConnectionStringsAsync(Guid id, SaasTenantConnectionStringsDto input)
    {
        input = await NormalizedConnectionStringsAsync(input);
        var etos = new List<TenantConnectionStringUpdatedEto>();
        using (var unitOfWork = UnitOfWorkManager.Begin(true, false))
        {
            var tenant = await TenantRepository.GetAsync(id);

            var str = tenant.FindDefaultConnectionString();
            if (input.Default != str)
            {
                etos.Add(new TenantConnectionStringUpdatedEto
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    ConnectionStringName = ConnectionStrings.DefaultConnectionStringName,
                    OldValue = str,
                    NewValue = input.Default
                });
                if (!input.Default.IsNullOrWhiteSpace())
                {
                    tenant.SetDefaultConnectionString(input.Default);
                }
                else
                {
                    tenant.RemoveDefaultConnectionString();
                }
            }

            if (input.Databases != null)
            {
                var toBeDeleted = tenant.ConnectionStrings.Where(x =>
                {
                    return x.Name != ConnectionStrings.DefaultConnectionStringName && input.Databases.All(d => x.Name != d.DatabaseName);
                }).ToList();
                etos.AddRange(toBeDeleted.Select(connectionString =>
                {
                    var tenantConnectionStringUpdatedEto = new TenantConnectionStringUpdatedEto
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        ConnectionStringName = connectionString.Name,
                        OldValue = connectionString.Value,
                        NewValue = null
                    };
                    return tenantConnectionStringUpdatedEto;
                }));
                tenant.ConnectionStrings.RemoveAll(x => toBeDeleted.Any(y => x.Name == y.Name));
                foreach (var databasis in input.Databases)
                {
                    etos.Add(new TenantConnectionStringUpdatedEto
                    {
                        Id = tenant.Id,
                        Name = tenant.Name,
                        ConnectionStringName = databasis.DatabaseName,
                        OldValue = tenant.FindConnectionString(databasis.DatabaseName),
                        NewValue = databasis.ConnectionString
                    });
                    tenant.SetConnectionString(databasis.DatabaseName, databasis.ConnectionString);
                }
            }

            await TenantRepository.UpdateAsync(tenant, false);
            await unitOfWork.CompleteAsync();
        }

        foreach (var tenantConnectionStringUpdatedEto in etos)
        {
            await DistributedEventBus.PublishAsync(tenantConnectionStringUpdatedEto);
        }
    }

    protected virtual Task<SaasTenantConnectionStringsDto> NormalizedConnectionStringsAsync(SaasTenantConnectionStringsDto input)
    {
        if (input == null)
        {
            input = new SaasTenantConnectionStringsDto();
        }
        else if (!input.Databases.IsNullOrEmpty())
        {
            input.Databases = (from x in input.Databases.Where(p => DbConnectionOptions.Databases.Any(d => d.Key == p.DatabaseName && d.Value.IsUsedByTenants))
                               where !x.ConnectionString.IsNullOrWhiteSpace()
                               group x by x.DatabaseName into x
                               select x.First()).ToList();
        }

        return Task.FromResult(input);
    }

    [Authorize(AbpSaasPermissions.Tenants.ManageConnectionStrings)]
    public virtual async Task ApplyDatabaseMigrationsAsync(Guid id)
    {
        await DistributedEventBus.PublishAsync(new ApplyDatabaseMigrationsEto
        {
            TenantId = id,
            DatabaseName = ConnectionStrings.DefaultConnectionStringName
        });
        foreach (var abpDatabaseInfo in DbConnectionOptions.Databases.Values)
        {
            if (abpDatabaseInfo.IsUsedByTenants)
            {
                await DistributedEventBus.PublishAsync(new ApplyDatabaseMigrationsEto
                {
                    TenantId = id,
                    DatabaseName = abpDatabaseInfo.DatabaseName
                });
            }
        }
    }

    public virtual async Task<List<EditionLookupDto>> GetEditionLookupAsync()
    {
        var editions = await EditionRepository.GetListAsync();

        return ObjectMapper.Map<List<Edition>, List<EditionLookupDto>>(editions);
    }

    [Authorize(AbpSaasPermissions.Tenants.SetPassword)]
    public virtual async Task SetPasswordAsync(Guid id, SaasTenantSetPasswordDto input)
    {
        await DistributedEventBus.PublishAsync(new UserPasswordChangeRequestedEto
        {
            TenantId = id,
            UserName = input.Username,
            Password = input.Password
        });
    }
}
