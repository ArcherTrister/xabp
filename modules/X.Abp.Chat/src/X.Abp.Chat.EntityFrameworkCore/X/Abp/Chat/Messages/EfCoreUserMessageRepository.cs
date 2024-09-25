// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Chat.Messages;

namespace X.Abp.Chat.EntityFrameworkCore.Messages;

public class EfCoreUserMessageRepository : EfCoreRepository<IChatDbContext, UserMessage, Guid>, IUserMessageRepository
{
    public EfCoreUserMessageRepository(IDbContextProvider<IChatDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<List<MessageWithDetails>> GetMessagesAsync(Guid userId, Guid targetUserId, int skipCount, int maxResultCount, CancellationToken cancellationToken = default)
    {
        var query = from chatUserMessage in await GetDbSetAsync()
                    join message in (await GetDbContextAsync()).ChatMessages on chatUserMessage.ChatMessageId equals message.Id
                    where userId == chatUserMessage.UserId && targetUserId == chatUserMessage.TargetUserId
                    select new MessageWithDetails
                    {
                        UserMessage = chatUserMessage,
                        Message = message
                    };

        return await query.OrderByDescending(x => x.Message.CreationTime).PageBy(skipCount, maxResultCount).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<bool> HasConversationAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).AnyAsync(userMessage => userMessage.UserId == userId && userMessage.TargetUserId == targetUserId, GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<UserMessage>> GetListAsync(Guid messageId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).Where(userMessage => userMessage.ChatMessageId == messageId).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<List<Guid>> GetAllMessageIdsAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync()).Where(userMessage => (userMessage.UserId == userId && userMessage.TargetUserId == targetUserId) || (userMessage.UserId == targetUserId && userMessage.TargetUserId == userId)).Select(userMessage => userMessage.ChatMessageId).ToListAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task<MessageWithDetails> GetLastMessageAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        IQueryable<UserMessage> userMessageQueryable = await GetDbSetAsync();
        IChatDbContext chatDbContext = await GetDbContextAsync();

        IQueryable<MessageWithDetails> source = userMessageQueryable.Join(chatDbContext.ChatMessages, userMessage => userMessage.ChatMessageId, message => message.Id, (userMessage, message) => new
        {
            chatUserMessage = userMessage,
            message = message
        }).Where(data => userId == data.chatUserMessage.UserId && targetUserId == data.chatUserMessage.TargetUserId)
        .Select(p => new MessageWithDetails { Message = p.message, UserMessage = p.chatUserMessage });

        return await source.OrderByDescending(messageWithDetails => messageWithDetails.Message.CreationTime).FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
    }

    public virtual async Task DeleteAllMessages(Guid userId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        await (await GetDbSetAsync()).Where(userMessage => (userMessage.UserId == userId && userMessage.TargetUserId == targetUserId) || (userMessage.UserId == targetUserId && userMessage.TargetUserId == userId)).ExecuteDeleteAsync(GetCancellationToken(cancellationToken));
    }
}
