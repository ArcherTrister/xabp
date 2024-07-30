// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

namespace X.Abp.Chat.Messages;

public class SendMessageInput
{
    public Guid TargetUserId { get; set; }

    [Required]
    [StringLength(ChatMessageConsts.MaxTextLength, MinimumLength = 1)]
    public string Message { get; set; }
}
