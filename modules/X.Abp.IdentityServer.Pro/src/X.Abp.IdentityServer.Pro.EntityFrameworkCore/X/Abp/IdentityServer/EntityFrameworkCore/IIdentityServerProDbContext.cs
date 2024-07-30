// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Data;
using Volo.Abp.IdentityServer;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

namespace X.Abp.IdentityServer.EntityFrameworkCore;

[IgnoreMultiTenancy]
[ConnectionStringName(AbpIdentityServerDbProperties.ConnectionStringName)]
public interface IIdentityServerProDbContext : IIdentityServerDbContext
{
}
