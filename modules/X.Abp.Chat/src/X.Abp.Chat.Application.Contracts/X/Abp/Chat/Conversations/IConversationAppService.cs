// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Services;

using X.Abp.Chat.Messages;

namespace X.Abp.Chat.Conversations;

public interface IConversationAppService : IApplicationService
{
    Task SendMessageAsync(SendMessageInput input);

    Task<ChatConversationDto> GetConversationAsync(GetConversationInput input);

    Task MarkConversationAsReadAsync(MarkConversationAsReadInput input);
}
