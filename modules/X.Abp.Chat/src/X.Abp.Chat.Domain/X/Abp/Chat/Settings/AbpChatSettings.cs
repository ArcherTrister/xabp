// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Chat.Settings;

public static class AbpChatSettings
{
    private const string Prefix = "X.Abp.Chat";

    public static class Messaging
    {
        private const string MessagingPrefix = Prefix + ".Messaging";

        public const string SendMessageOnEnter = MessagingPrefix + ".SendMessageOnEnter";

        public const string DeletingMessages = MessagingPrefix + ".DeletingMessages";
        public const string DeletingConversations = MessagingPrefix + ".DeletingConversations";
        public const string MessageDeletionPeriod = MessagingPrefix + ".MessageDeletionPeriod";
    }
}
