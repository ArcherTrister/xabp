// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Modularity;

using X.Abp.LanguageManagement.EntityFrameworkCore;

namespace X.Abp.LanguageManagement;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(LanguageManagementEntityFrameworkCoreTestModule),
    typeof(LanguageManagementTestBaseModule))]
public class LanguageManagementDomainTestModule : AbpModule
{
}
