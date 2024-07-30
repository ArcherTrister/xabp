// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

namespace X.Abp.Chat.Conversations;

public class GetConversationInput : PagedResultRequestDto
{
    public Guid TargetUserId { get; set; }
}
