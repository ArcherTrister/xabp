// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore;

public class MyProjectNameDbContextFactory :
    MyProjectNameDbContextFactoryBase<MyProjectNameDbContext>
{
    protected override MyProjectNameDbContext CreateDbContext(
        DbContextOptions<MyProjectNameDbContext> dbContextOptions)
    {
        return new MyProjectNameDbContext(dbContextOptions);
    }
}
