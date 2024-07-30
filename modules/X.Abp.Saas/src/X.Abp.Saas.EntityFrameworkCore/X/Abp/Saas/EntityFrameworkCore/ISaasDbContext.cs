// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

using X.Abp.Saas.Editions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas.EntityFrameworkCore;

[ConnectionStringName(SaasDbProperties.ConnectionStringName)]
[IgnoreMultiTenancy]
public interface ISaasDbContext : IEfCoreDbContext
{
    DbSet<Tenant> Tenants { get; }

    DbSet<Edition> Editions { get; }

    DbSet<TenantConnectionString> TenantConnectionStrings { get; }
}
