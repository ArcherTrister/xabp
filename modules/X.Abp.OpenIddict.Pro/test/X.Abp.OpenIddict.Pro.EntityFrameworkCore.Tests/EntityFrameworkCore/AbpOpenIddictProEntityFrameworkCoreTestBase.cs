// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.OpenIddict.EntityFrameworkCore;

/* This class can be used as a base class for EF Core integration tests,
 * while SampleRepository_Tests uses a different approach.
 */
public abstract class AbpOpenIddictProEntityFrameworkCoreTestBase : AbpOpenIddictProTestBase<AbpOpenIddictProEntityFrameworkCoreTestModule>
{
}
