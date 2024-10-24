// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.OpenIddict.EntityFrameworkCore;

namespace X.Abp.OpenIddict.EntityFrameworkCore;

public static class OpenIddictProDbContextModelBuilderExtensions
{
    public static void ConfigureOpenIddictPro(
        this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.ConfigureOpenIddict();
        builder.TryConfigureObjectExtensions<OpenIddictProDbContext>();
    }
}
