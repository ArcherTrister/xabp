﻿using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.FileManagement.MongoDB;

public static class FileManagementMongoDbContextExtensions
{
    public static void ConfigureFileManagement(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
