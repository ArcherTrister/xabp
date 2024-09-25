// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using X.Abp.Chat.Messages;

namespace X.Abp.Chat.Settings;
public class ChatSettingsDto
{
    [Range(1, 3)]
    public ChatDeletingMessages DeletingMessages { get; set; }

    [Range(0, int.MaxValue)]
    public int MessageDeletionPeriod { get; set; }

    [Range(1, 2)]
    public ChatDeletingConversations DeletingConversations { get; set; }
}
