using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.TextTemplateManagement.MongoDB;

public static class TextTemplateManagementMongoDbContextExtensions
{
    public static void ConfigureTextTemplateManagement(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
