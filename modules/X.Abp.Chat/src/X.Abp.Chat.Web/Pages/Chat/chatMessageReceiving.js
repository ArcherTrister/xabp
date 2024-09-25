(function ($) {

    abp.event.on('abp.serviceProxyScriptInitialized', function () {
        $(function () {

            if (abp.currentUser && abp.currentUser.id && abp.auth.isGranted('Chat.Messaging')) {
                var l = abp.localization.getResource('Chat');
                var isChatPageOpen = $('#chat_wrapper').length > 0;

                if (!isChatPageOpen) {
                    volo.chat.users.contact.getTotalUnreadMessageCount({}).then(function (result) {
                        addUnreadMessageToChatIcon(result);
                    });
                }

                var addUnreadMessageToChatIcon = function (count = 1) {
                    var chatIconContainer = $('#lpx-toolbar').find('a[href="/Chat"]');
                    var span = $(chatIconContainer).find('span');

                    if (span && span.length > 0) {
                        var unReadMessageSpan = span.find("span.unread-message");
                        if (unReadMessageSpan && unReadMessageSpan.length > 0) {
                            var prevCount = parseInt(unReadMessageSpan.data("count").trim("+"));
                            span.html(getUnReadMessageSpan(prevCount + count));
                        }
                        else if (count > 0) {
                            chatIconContainer.append(getUnReadMessageSpan(count));
                        }
                    }
                };

                var getUnReadMessageSpan = function (count) {
                    var messageCount = count >= 50 ? "50+" : count;

                    return (
                        '<span class="unread-message badge bg-primary" data-count="' + messageCount + '" style="position: absolute; right: 1rem; display: block; font-size: 0.5rem;"> ' +
                        messageCount +
                        ' </span>'
                    );
                }

                var connection = new signalR.HubConnectionBuilder().withUrl("/signalr-hubs/chat").build();

                connection.on("ReceiveMessage", function (message) {
                    if (isChatPageOpen) {
                        $(document).trigger("ChatMessageReceived", message);
                    } else {
                        addUnreadMessageToChatIcon();

                        var senderTitle = message.senderName && message.senderName != '' ?
                            message.senderName + (message.senderSurname ? ' ' + message.senderSurname : '')
                            : message.senderUsername;

                        var shortMessage = message.text.length > 50 ? message.text.substring(0, 49) + '...' : message.text;

                        abp.notify.info(senderTitle + ': ' + shortMessage, l('NewChatMessage'), {
                            onclick: function () {
                                window.location.replace("/chat?conversation_id=" + message.senderUserId);
                            }
                        });
                    }
                });

                connection.on("DeleteMessage", function (messageId) {
                    if (isChatPageOpen) {
                        $(document).trigger("ChatDeleteMessage", messageId);
                    }
                });

                connection.on("DeleteConversation", function (userId) {
                    if (isChatPageOpen) {
                        $(document).trigger("ChatDeleteConversation", userId);
                    }
                });

                connection.start().then(function () {
                })
                    .catch(function (err) {
                        return console.error(err.toString());
                    });

            }
        });

    });
})(jQuery);
