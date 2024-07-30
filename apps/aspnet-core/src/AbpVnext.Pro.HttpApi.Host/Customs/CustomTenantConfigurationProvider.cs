// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace AbpVnext.Pro.Customs;

public class CustomTenantConfigurationProvider : ITenantConfigurationProvider, ITransientDependency
{
    protected virtual ITenantResolver TenantResolver { get; }

    protected virtual ITenantStore TenantStore { get; }

    protected virtual ITenantResolveResultAccessor TenantResolveResultAccessor { get; }

    // protected virtual IStringLocalizer<AbpMultiTenancyResource> StringLocalizer { get; }
    public CustomTenantConfigurationProvider(
        ITenantResolver tenantResolver,
        ITenantStore tenantStore,
        ITenantResolveResultAccessor tenantResolveResultAccessor)
    {
        TenantResolver = tenantResolver;
        TenantStore = tenantStore;
        TenantResolveResultAccessor = tenantResolveResultAccessor;
    }

    public virtual async Task<TenantConfiguration> GetAsync(bool saveResolveResult = false)
    {
        var resolveResult = await TenantResolver.ResolveTenantIdOrNameAsync();

        if (saveResolveResult)
        {
            TenantResolveResultAccessor.Result = resolveResult;
        }

        TenantConfiguration tenant = null;
        if (resolveResult.TenantIdOrName != null)
        {
            tenant = await FindTenantAsync(resolveResult.TenantIdOrName);

            if (tenant == null)
            {
                throw new BusinessException(
                    code: "Volo.AbpIo.MultiTenancy:010001",
                    message: "Tenant not found!",
                    details: "There is no tenant with the tenant id or name: " + resolveResult.TenantIdOrName);
            }

            if (!tenant.IsActive)
            {
                throw new BusinessException(
                    code: "Volo.AbpIo.MultiTenancy:010002",
                    message: "Tenant not active!",
                    details: "The tenant is no active with the tenant id or name: " + resolveResult.TenantIdOrName);
            }

            /*
            //if (tenant == null)
            //{
            //    throw new BusinessException(
            //        code: "Volo.AbpIo.MultiTenancy:010001",
            //        message: StringLocalizer["TenantNotFoundMessage"],
            //        details: StringLocalizer["TenantNotFoundDetails", resolveResult.TenantIdOrName]);
            //}

            //if (!tenant.IsActive)
            //{
            //    throw new BusinessException(
            //        code: "Volo.AbpIo.MultiTenancy:010002",
            //        message: StringLocalizer["TenantNotActiveMessage"],
            //        details: StringLocalizer["TenantNotActiveDetails", resolveResult.TenantIdOrName]);
            //}
            */
        }

        return tenant;
    }

    protected virtual async Task<TenantConfiguration> FindTenantAsync(string tenantIdOrName)
    {
        return Guid.TryParse(tenantIdOrName, out var parsedTenantId)
            ? await TenantStore.FindAsync(parsedTenantId)
            : await TenantStore.FindAsync(tenantIdOrName);
    }
}
