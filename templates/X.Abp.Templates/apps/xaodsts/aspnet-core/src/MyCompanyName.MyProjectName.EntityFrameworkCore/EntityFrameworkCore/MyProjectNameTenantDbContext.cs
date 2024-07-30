// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace MyCompanyName.MyProjectName.EntityFrameworkCore
{
    [ConnectionStringName("Default")]
    public class MyProjectNameTenantDbContext : MyProjectNameDbContextBase<MyProjectNameTenantDbContext>
    {
        public MyProjectNameTenantDbContext(DbContextOptions<MyProjectNameTenantDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.SetMultiTenancySide(MultiTenancySides.Tenant);

            base.OnModelCreating(modelBuilder);
        }
    }
}