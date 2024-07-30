using Volo.Abp;
using Volo.Abp.MongoDB;

namespace X.Abp.Chat.MongoDB;

public static class ChatMongoDbContextExtensions
{
    public static void ConfigureChat(
        this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));
    }
}
