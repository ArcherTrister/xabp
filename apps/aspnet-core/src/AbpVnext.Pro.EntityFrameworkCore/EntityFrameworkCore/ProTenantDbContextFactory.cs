// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

namespace AbpVnext.Pro.EntityFrameworkCore;

public class ProTenantDbContextFactory :
    ProDbContextFactoryBase<ProTenantDbContext>
{
    protected override ProTenantDbContext CreateDbContext(
        DbContextOptions<ProTenantDbContext> dbContextOptions)
    {
        return new ProTenantDbContext(dbContextOptions);
    }
}
