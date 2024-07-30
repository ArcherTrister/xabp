// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.IdentityServer.EntityFrameworkCore;

namespace X.Abp.IdentityServer.EntityFrameworkCore;

public static class IdentityServerProDbContextModelBuilderExtensions
{
    public static void ConfigureIdentityServerPro(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
        builder.ConfigureIdentityServer();
        builder.TryConfigureObjectExtensions<IdentityServerProDbContext>();
    }
}
