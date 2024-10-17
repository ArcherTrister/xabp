// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.Abp.MongoDB;
using Volo.Abp.Users.MongoDB;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.Messages;
using X.Abp.Chat.MongoDB.Messages;
using X.Abp.Chat.MongoDB.Users;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.MongoDB;

[DependsOn(
    typeof(AbpChatDomainModule),
    typeof(AbpUsersMongoDbModule),
    typeof(AbpMongoDbModule))]
public class ChatMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<ChatMongoDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, MongoQuestionRepository>();
             */
            options.AddRepository<Message, MongoMessageRepository>();
            options.AddRepository<UserMessage, MongoUserMessageRepository>();
            options.AddRepository<ChatUser, MongoChatUserRepository>();
            options.AddRepository<Conversation, MongoConversationRepository>();
        });
    }
}
