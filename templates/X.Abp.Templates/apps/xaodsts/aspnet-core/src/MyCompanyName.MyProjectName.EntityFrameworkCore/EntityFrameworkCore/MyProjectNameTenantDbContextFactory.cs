// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore;

public class MyProjectNameTenantDbContextFactory :
    MyProjectNameDbContextFactoryBase<MyProjectNameTenantDbContext>
{
    protected override MyProjectNameTenantDbContext CreateDbContext(
        DbContextOptions<MyProjectNameTenantDbContext> dbContextOptions)
    {
        return new MyProjectNameTenantDbContext(dbContextOptions);
    }
}
