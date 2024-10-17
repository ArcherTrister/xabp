// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.MongoDB;

namespace X.Abp.Identity.MongoDB;

public static class IdentityProMongoDbContextExtensions
{
    public static void ConfigureIdentityPro(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.ConfigureIdentity();
    }
}
