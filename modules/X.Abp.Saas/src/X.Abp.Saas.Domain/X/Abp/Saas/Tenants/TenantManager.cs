// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Saas.Tenants;

public class TenantManager : DomainService, ITenantManager, ITransientDependency, IDomainService
{
    protected ITenantRepository TenantRepository { get; }

    protected ITenantNormalizer TenantNormalizer { get; }

    protected ILocalEventBus LocalEventBus { get; }

    public TenantManager(
      ITenantRepository tenantRepository,
      ITenantNormalizer tenantNormalizer,
      ILocalEventBus localEventBus)
    {
        TenantRepository = tenantRepository;
        TenantNormalizer = tenantNormalizer;
        LocalEventBus = localEventBus;
    }

    public virtual async Task<Tenant> CreateAsync(string name, Guid? editionId = null)
    {
        Check.NotNull(name, nameof(name));
        string normalizedName = TenantNormalizer.NormalizeName(name);
        await ValidateNameAsync(normalizedName);
        return new Tenant(GuidGenerator.Create(), name, normalizedName, editionId);
    }

    public virtual async Task ChangeNameAsync(Tenant tenant, string name)
    {
        Check.NotNull(tenant, nameof(tenant));
        Check.NotNull(name, nameof(name));
        string normalizedName = TenantNormalizer.NormalizeName(name);
        await ValidateNameAsync(normalizedName, tenant.Id);
        await LocalEventBus.PublishAsync(new TenantChangedEvent(tenant.Id, tenant.NormalizedName), true);
        tenant.SetName(name);
        tenant.SetNormalizedName(normalizedName);
    }

    protected virtual async Task ValidateNameAsync(string normalizeName, Guid? expectedId = null)
    {
        var tenant = await TenantRepository.FindByNameAsync(normalizeName);
        if (tenant != null && tenant.Id != expectedId)
        {
            throw new BusinessException("X.Abp.Saas:DuplicateTenantName").WithData("Name", normalizeName);
        }
    }

    public virtual Task<bool> IsActiveAsync(Tenant tenant)
    {
        var flag = tenant.ActivationState switch
        {
            TenantActivationState.Active => true,
            TenantActivationState.ActiveWithLimitedTime => tenant.ActivationEndDate >= Clock.Now,
            TenantActivationState.Passive => false,
            _ => false,
        };
        return Task.FromResult(flag);
    }
}
