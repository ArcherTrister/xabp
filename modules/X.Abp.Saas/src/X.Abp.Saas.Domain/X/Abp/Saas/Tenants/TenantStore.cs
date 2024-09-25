// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace X.Abp.Saas.Tenants;

public class TenantStore : ITenantStore, ITransientDependency
{
    protected ITenantRepository TenantRepository { get; }

    protected IObjectMapper<AbpSaasDomainModule> ObjectMapper { get; }

    protected ICurrentTenant CurrentTenant { get; }

    protected IDistributedCache<TenantConfigurationCacheItem> Cache { get; }

    protected ITenantManager TenantManager { get; }

    public TenantStore(
      ITenantRepository tenantRepository,
      IObjectMapper<AbpSaasDomainModule> objectMapper,
      ICurrentTenant currentTenant,
      IDistributedCache<TenantConfigurationCacheItem> cache,
      ITenantManager tenantManager)
    {
        TenantRepository = tenantRepository;
        ObjectMapper = objectMapper;
        CurrentTenant = currentTenant;
        Cache = cache;
        TenantManager = tenantManager;
    }

    public virtual async Task<TenantConfiguration> FindAsync(
      string normalizedName)
    {
        return (await GetCacheItemAsync(null, normalizedName)).Value;
    }

    public virtual async Task<TenantConfiguration> FindAsync(Guid id)
    {
        return (await GetCacheItemAsync(id, null)).Value;
    }

    public virtual async Task<IReadOnlyList<TenantConfiguration>> GetListAsync(
      bool includeDetails = false)
    {
        return ObjectMapper.Map<List<Tenant>, List<TenantConfiguration>>(await TenantRepository.GetListAsync(includeDetails));
    }

    [Obsolete("Use FindAsync method.")]
    public virtual TenantConfiguration Find(string normalizedName)
    {
        throw new NotImplementedException();
    }

    [Obsolete("Use FindAsync method.")]
    public virtual TenantConfiguration Find(Guid id)
    {
        throw new NotImplementedException();
    }

    protected virtual async Task<TenantConfigurationCacheItem> GetCacheItemAsync(
      Guid? id,
      string normalizedName)
    {
        string cacheKey = CalculateCacheKey(id, normalizedName);
        TenantConfigurationCacheItem cacheItem = await Cache.GetAsync(cacheKey, null, true);
        if (cacheItem?.Value != null)
        {
            return cacheItem;
        }

        if (id.HasValue)
        {
            using (CurrentTenant.Change(null))
            {
                Tenant tenant = await TenantRepository.FindAsync(id.Value, true);
                return await SetCacheAsync(cacheKey, tenant);
            }
        }
        else
        {
            if (normalizedName.IsNullOrWhiteSpace())
            {
                throw new AbpException("Both id and name can't be invalid.");
            }

            using (CurrentTenant.Change(null))
            {
                Tenant tenant = await TenantRepository.FindByNameAsync(normalizedName);
                return await SetCacheAsync(cacheKey, tenant);
            }
        }
    }

    protected virtual async Task<TenantConfigurationCacheItem> SetCacheAsync(
      string cacheKey,
      Tenant tenant)
    {
        TenantConfiguration tenantConfiguration = tenant != null ? ObjectMapper.Map<Tenant, TenantConfiguration>(tenant) : null;
        if (tenantConfiguration != null)
        {
            await TenantManager.IsActiveAsync(tenant);
        }

        TenantConfigurationCacheItem cacheItem = new TenantConfigurationCacheItem(tenantConfiguration);

        await Cache.SetAsync(cacheKey, cacheItem, null, null, true);

        return cacheItem;
    }

    [Obsolete("Use GetCacheItemAsync method.")]
    protected virtual TenantConfigurationCacheItem GetCacheItem(
      Guid? id,
      string normalizedName)
    {
        throw new NotImplementedException();
    }

    [Obsolete("Use SetCacheAsync method.")]
    protected virtual TenantConfigurationCacheItem SetCache(
      string cacheKey,
      Tenant tenant)
    {
        throw new NotImplementedException();
    }

    protected virtual string CalculateCacheKey(Guid? id, string normalizedName)
    {
        return TenantConfigurationCacheItem.CalculateCacheKey(id, normalizedName);
    }
}
