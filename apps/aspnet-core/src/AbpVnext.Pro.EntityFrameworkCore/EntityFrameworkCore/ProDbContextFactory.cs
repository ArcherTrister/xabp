// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

namespace AbpVnext.Pro.EntityFrameworkCore;

public class ProDbContextFactory :
    ProDbContextFactoryBase<ProDbContext>
{
    protected override ProDbContext CreateDbContext(
        DbContextOptions<ProDbContext> dbContextOptions)
    {
        return new ProDbContext(dbContextOptions);
    }
}
