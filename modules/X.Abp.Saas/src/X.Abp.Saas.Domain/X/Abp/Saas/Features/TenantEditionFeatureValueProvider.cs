// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;

using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.Features;

public class TenantEditionFeatureValueProvider : FeatureManagementProvider, ITransientDependency
{
    public const string ProviderName = "TE";

    public override string Name => ProviderName;

    protected ICurrentTenant CurrentTenant { get; }

    protected ITenantRepository TenantRepository { get; }

    public TenantEditionFeatureValueProvider(IFeatureManagementStore store, ICurrentTenant currentTenant, ITenantRepository tenantRepository)
        : base(store)
    {
        CurrentTenant = currentTenant;
        TenantRepository = tenantRepository;
    }

    public override bool Compatible(string providerName)
    {
        return providerName == ProviderName || Compatible(providerName);
    }

    public override async Task<string> GetOrNullAsync(FeatureDefinition feature, string providerKey)
    {
        var text = await NormalizeProviderKeyAsync(providerKey);
        return await Store.GetOrNullAsync(feature.Name, ProviderName, text);
    }

    public override async Task SetAsync(FeatureDefinition feature, string value, string providerKey)
    {
        var text = await NormalizeProviderKeyAsync(providerKey);
        await Store.SetAsync(feature.Name, value, ProviderName, text);
    }

    public override async Task ClearAsync(FeatureDefinition feature, string providerKey)
    {
        var text = await NormalizeProviderKeyAsync(providerKey);
        await Store.DeleteAsync(feature.Name, ProviderName, text);
    }

    protected override async Task<string> NormalizeProviderKeyAsync(string providerKey)
    {
        providerKey ??= CurrentTenant.Id?.ToString();

        return !Guid.TryParse(providerKey, out var result)
            ? null
            : (await TenantRepository.FindByIdAsync(result))?.GetActiveEditionId()?.ToString();
    }
}
