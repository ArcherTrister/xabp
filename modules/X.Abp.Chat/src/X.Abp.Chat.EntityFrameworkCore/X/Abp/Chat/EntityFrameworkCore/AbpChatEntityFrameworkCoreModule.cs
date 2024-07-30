// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Modularity;
using Volo.Abp.Users.EntityFrameworkCore;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.EntityFrameworkCore.Conversations;
using X.Abp.Chat.EntityFrameworkCore.Messages;
using X.Abp.Chat.EntityFrameworkCore.Users;
using X.Abp.Chat.Messages;
using X.Abp.Chat.Users;

namespace X.Abp.Chat.EntityFrameworkCore;

[DependsOn(
    typeof(AbpChatDomainModule),
    typeof(AbpUsersEntityFrameworkCoreModule),
    typeof(AbpEntityFrameworkCoreModule))]
public class AbpChatEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<ChatDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, EfCoreQuestionRepository>();
             */
            options.AddDefaultRepositories<IChatDbContext>();

            options.AddRepository<Message, EfCoreMessageRepository>();
            options.AddRepository<UserMessage, EfCoreUserMessageRepository>();
            options.AddRepository<ChatUser, EfCoreChatUserRepository>();
            options.AddRepository<Conversation, EfCoreConversationRepository>();
        });
    }
}
