// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

using X.Abp.Saas.Editions;

namespace AbpVnext.Pro.Saas;

public class SaasDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IEditionDataSeeder _editionDataSeeder;
    private readonly ICurrentTenant _currentTenant;

    public SaasDataSeedContributor(IEditionDataSeeder editionDataSeeder, ICurrentTenant currentTenant)
    {
        _editionDataSeeder = editionDataSeeder;
        _currentTenant = currentTenant;
    }

    [UnitOfWork]
    public virtual async Task SeedAsync(DataSeedContext context)
    {
        using (_currentTenant.Change(context?.TenantId))
        {
            await _editionDataSeeder.CreateStandardEditionsAsync();
        }
    }
}
