// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Modularity;

using X.Abp.CmsKit.Pro.EntityFrameworkCore;

namespace X.Abp.CmsKit.Pro;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(CmsKitProEntityFrameworkCoreTestModule),
    typeof(CmsKitProTestBaseModule))]
public class CmsKitProDomainTestModule : AbpModule
{
}
