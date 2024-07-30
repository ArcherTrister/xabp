// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace X.Abp.LanguageManagement.Data;

public class LanguageManagementDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly LanguageManagementDataSeeder _languageManagementDataSeeder;

    public LanguageManagementDataSeedContributor(LanguageManagementDataSeeder languageManagementDataSeeder)
    {
        _languageManagementDataSeeder = languageManagementDataSeeder;
    }

    public virtual async Task SeedAsync(DataSeedContext context)
    {
        if (context.TenantId != null)
        {
            /* Language is not multi-tenant */
            return;
        }

        await _languageManagementDataSeeder.SeedAsync();
    }
}
