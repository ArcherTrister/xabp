(function ($) {

    var l = abp.localization.getResource('Chat');
    var _conversationService = volo.chat.conversations.conversation;
    var _contactService = volo.chat.users.contact;
    var _settingService = volo.chat.settings.settings;
    var _wrapper = $('#chat_wrapper');

    var searchingPermission = abp.auth.isGranted("Chat.Searching");
    if (searchingPermission) {
        $("#SearchBox").show();
    }
    var pageSize = 50;

    const ChatDeletingMessages = {
        enabled: "Enabled",
        disabled: "Disabled",
        enabledWithDeletionPeriod: "EnabledWithDeletionPeriod"
    }

    const ChatDeletingConversations = {
        enabled: "Enabled",
        disabled: "Disabled"
    }

    var queryStringParser = {
        keyValueArray: [],
        getParameterValue: function (parameterName) {
            var queryStringParsed = this.parseToKeyValueArray();

            for (var i = 0; i < queryStringParsed.length; i++) {
                if (queryStringParsed[i].key == parameterName) {
                    return queryStringParsed[i].value;
                }
            }

            return null;
        },
        parseToKeyValueArray: function () {
            if (this.keyValueArray.length === 0 && location.search !== '') {
                var parametersAndValues = location.search.substring(1).split('&');

                for (var i = 0; i < parametersAndValues.length; i++) {
                    var parameterAndValue = parametersAndValues[i].split('=');
                    this.keyValueArray.push({ key: parameterAndValue[0], value: parameterAndValue[1] });
                }
            }

            return this.keyValueArray;
        }
    };

    var dateManager = {
        months: ['', 'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sept', 'Oct', 'Nov', 'Dec'],
        isToday: function (date) {
            if (date.getFullYear() === (new Date()).getFullYear() &&
                date.getMonth() === (new Date()).getMonth() &&
                date.getDate() === (new Date()).getDate()) {
                return true;
            }
            return false;
        },
        formatMessageDate: function (dateAsString) {
            if (dateAsString === null) {
                return '';
            }

            var date = new Date(dateAsString);

            var formattedTime = (date.getHours() > 9 ? date.getHours() : '0' + date.getHours()) + ':' +
                (date.getMinutes() > 9 ? date.getMinutes() : '0' + date.getMinutes());

            if (!dateManager.isToday(date)) {

                var year = date.getFullYear() == (new Date()).getFullYear() ? '' : ' ' + date.getFullYear();
                var month = (1 + date.getMonth()).toString();
                var day = date.getDate().toString();

                var formattedDate = day + ' ' + l(dateManager.months[month]) + year;
                formattedTime += ' ' + formattedDate;
            }

            return formattedTime;
        },
        formatContactDate: function (dateAsString) {
            if (dateAsString === null) {
                return '';
            }

            var date = new Date(dateAsString);

            if (!dateManager.isToday(date)) {
                var year = date.getFullYear() == (new Date()).getFullYear() ? '' : date.getFullYear();
                var month = (1 + date.getMonth()).toString();
                var day = date.getDate().toString();

                return day + ' ' + l(dateManager.months[month]) + ' ' + year;
            } else {
                return (date.getHours() > 9 ? date.getHours() : '0' + date.getHours()) + ':' +
                    (date.getMinutes() > 9 ? date.getMinutes() : '0' + date.getMinutes());
            }
        }
    };

    var colorGenerator = {
        colors: [
            { text: '#ffffff', background: '#3cb160' },
            { text: '#ffffff', background: '#c373cc' },
            { text: '#ffffff', background: '#2b78b3' },
            { text: '#ffffff', background: '#6ac79a' },
            { text: '#ffffff', background: '#aeb140' },
            { text: '#ffffff', background: '#b773c0' },
            { text: '#ffffff', background: '#e16d7a' },
            { text: '#ffffff', background: '#ffac2a' },
            { text: '#ffffff', background: '#21bbc7' },
            { text: '#ffffff', background: '#59ab95' }
        ],
        generateFromString: function (str) {
            var hash = 0;
            for (var i = 0; i < str.length; i++) {
                hash = str.charCodeAt(i) + ((hash << 5) - hash);
            }
            return colorGenerator.colors[Math.abs(hash % 10)];
        }
    };

    var avatarManager = {
        init: {},
        createCanvasForUser: function (canvas, username, name) {
            var hashText;
            var text;

            if (name && name.length > 0) {
                hashText = name;

                var nameSplited = name.trim().split(" ");

                if (nameSplited.length >= 2) {
                    var firstName = nameSplited[0];
                    var lastName = nameSplited[nameSplited.length - 1];

                    text = firstName.length >= 1 ? firstName.substring(0, 1) : firstName;
                    text += lastName.length >= 1 ? lastName.substring(0, 1) : lastName;
                } else {
                    text = name.length >= 2 ? name.substring(0, 2) : name;
                }
            } else {
                hashText = username;
                text = username && username.length >= 2 ? username.substring(0, 2) : username;
            }

            var colors = colorGenerator.generateFromString(hashText);

            var ctx = canvas.getContext("2d");

            ctx.fillStyle = colors.background;
            ctx.fillRect(0, 0, canvas.width, canvas.height);

            ctx.font = "bold 15px Arial";
            ctx.fillStyle = colors.text;
            ctx.fillText(text.toUpperCase().substring(0, 2), canvas.width / 2 - 10, canvas.height / 2 + 5);
        }
    };

    var contacts = {
        conversations: {},
        init: function () {
            contacts.setContactFilterListener();
            contacts.refresh(contacts.focusOnAContact);
            contacts.setContactChangeListener();
        },
        setContactChangeListener: function () {
            $(document).on('click', '.chat-contact', function (e) {
                var userId = $(this).data('userid');
                var username = $(this).data('username');
                var conversationTitle = $(this).data('conversation-title');
                var isNewContact = $(this).data('is-new-contact');
                var hasChatPermission = $(this).data('has-chat-permission');

                _wrapper.find('#Send_Message_Form #userId').val(userId);
                _wrapper.find('#Conversation_Title').html(conversationTitle);
                avatarManager.createCanvasForUser(_wrapper.find('#Conversation_Avatar')[0], username, conversationTitle);

                if (isNewContact == false) {
                    var unreadMessageCount = parseInt($(this).find('.bg-success').html().trim());
                    conversation.loadData(userId, unreadMessageCount);
                } else {
                    conversation.clear();
                }

                contacts.setActiveContact(this);

                var chatBox = $("#chatBox");
                if (!hasChatPermission) {
                    chatBox.css("pointer-events", "none");
                    chatBox.css("opacity", "0.4");
                    var chatBoxTitle = $("#Conversation_Title");
                    chatBoxTitle.text(chatBoxTitle.text() + " ¡ª " + l('Volo.Chat:010004'));
                } else {
                    chatBox.css("pointer-events", "inherit");
                    chatBox.css("opacity", "inherit");
                }

                _wrapper.find('#AvatarId').show();
                $('#EmptyTemplate').hide();
                $('#chat_conversation_wrapper').show();
                $('#Send_Message_Form').show();
                _wrapper.find('#Chat_Message_Box').val('');
                _wrapper.find('#Chat_Message_Box').focus();
                _wrapper.find('#Send_Message_Form #Send_Message_Button').prop('disabled', true);
            });
        },
        focusOnAContact: function () {
            var targetConversationId = queryStringParser.getParameterValue('conversation_id');
            if (targetConversationId) {
                _wrapper.find('.chat-contact[data-userid=' + targetConversationId + ']').trigger('click');
            } else {
                $(_wrapper.find('.chat-contact')[0]).trigger('click');
            }
        },
        setActiveContact: function (activeContact) {
            var oldActiveContact = _wrapper.find('.chat-contact, .active');
            if (oldActiveContact.length > 1) {
                $(oldActiveContact).removeClass('active');
            }
            $(activeContact).addClass('active');
        },
        addCanvasToContacts: function () {
            var contacts = _wrapper.find('.chat-contact');

            for (var i = 0; i < contacts.length; i++) {
                var canvas = $(contacts[i]).find('.canvas-avatar')[0];
                var username = $(contacts[i]).data('username');
                var name = $(contacts[i]).data('conversation-title');
                avatarManager.createCanvasForUser(canvas, username, name);
            }
        },
        refresh: function (callback) {
            let filter = _wrapper.find('#Contacts_Filter').val();
            if (filter.length > 1 && filter[0] === ' ' && filter[1] !== ' ') {
                filter = filter.trim();
                _wrapper.find('#Contacts_Filter').val(filter);
            }
            let includeOtherContacts = filter !== null && filter !== '';
            _contactService.getContacts({
                filter: _wrapper.find('#Contacts_Filter').val(),
                includeOtherContacts: includeOtherContacts
            }).then(function (result) {
                let contactsAsHtml = '';
                let otherContact = false;
                for (var i = 0; i < result.length; i++) {
                    if (otherContact === false &&
                        (result[i].lastMessage === null || result[i].lastMessage === '')) {
                        contactsAsHtml += contacts.getSeparatorTemplate();
                        otherContact = true;
                    }
                    contactsAsHtml += contacts.getContactTemplate(result[i], otherContact);
                }

                if (includeOtherContacts === true || result.length > 0) {
                    $('#EmptyTemplate').hide();
                    $('#chat_conversation_wrapper').show();
                    $('#Send_Message_Form').show();
                    _wrapper.find('#AvatarId').show();
                }
                else {
                    $('#EmptyTemplate').show();
                    $('#chat_conversation_wrapper').hide();
                    $('#Send_Message_Form').hide();
                    _wrapper.find('#Conversation_Title').html('');
                    _wrapper.find('#AvatarId').hide();
                }

                _wrapper.find('#contact_list').empty();
                _wrapper.find('#contact_list').html(contactsAsHtml);

                contacts.addCanvasToContacts();
                conversation.bindDeleteConversationEvent();
                if (callback) {
                    callback();
                }
            });
        },
        shortenMessage: function (message) {
            return message.length > 24 ? message.substring(0, 23) + '...' : message;
        },
        getContactTemplate: function (contact, hideDate) {
            if (contact.lastMessage === null) {
                contact.lastMessage = '';
            }

            var lastMessageDate = contact.lastMessageDate === null || hideDate ? '' : dateManager.formatContactDate(contact.lastMessageDate);
            var isNewContact = contact.lastMessage === null || contact.lastMessage === '';

            var unreadBadge = '';

            if (contact.lastMessageSide === 2 && contact.unreadMessageCount > 0) {
                unreadBadge = '<small class="badge bg-success me-1">' + contact.unreadMessageCount + '</small>';
            } else {
                unreadBadge = '<small class="badge bg-success me-1" style="display:none">0</small>';
            }

            var deletingTemplate = "";
            if (conversation.isDeletingConversationEnabled()) {
                deletingTemplate = "<div class=\"dropdown position-absolute bottom-0 end-0\">" +
                    "<button class=\"btn p-1\" type=\"button\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">" +
                    "<i class=\"lpx-caret bi-chevron-down\" aria-hidden=\"true\"></i>" +
                    "</button>" +
                    "<ul class=\"dropdown-menu\">" +
                    "<li><a class=\"dropdown-item delete-conversation-btn\" data-id=" + contact.userId + " href=\"#\">" + l("Delete") + "</a></li>" +
                    "</ul>" +
                    "</div>";
            }

            var template = "\<div class=\"active border-2 chat-contact cursor-pointer list-group-item list-group-item-action mb-1 mt-0 px-2 rounded-2\" data-conversation-title=\"" + contacts.getName(contact) + "\" data-userid=\"" + contact.userId + "\" data-username=\"" + contact.username + "\" data-is-new-contact=" + isNewContact + " data-has-chat-permission=\"" + contact.hasChatPermission + "\"" + ">" +
                "<div class=\"media \">" +
                //"<img src=\"https://avatars.dicebear.com/v2/human/voloqq.svg\" alt=\"user\" width=\"44\" class=\"rounded-circle mx-auto me-md-2 float-start \">" +
                "<canvas class=\"canvas-avatar me-2 float-start rounded-circle\" width=\"48\" height=\"48\"> .</canvas >" +
                "<div class=\"media-body ms-2\">" +
                "<div class=\"d-flex align-items-center justify-content-between mb-0\">" +
                "<h5 class=\"mb-0 mt-1\">" +
                unreadBadge + contacts.getName(contact) + "</h5> <small class=\"last-message-date\">" + lastMessageDate + "</small>" +
                "</div > " +
                "<p class=\"mb-0 small last-message\">" + contacts.shortenMessage(contact.lastMessage) + "</p>" +
                "</div > " +
                "</div > " +
                deletingTemplate +
                "</div>";

            return template;
        },
        getName: function (contact) {
            var name = '';

            if (contact.name !== null && contact.name !== '') {
                name += contact.name + ' ';
            }
            if (contact.surname !== null && contact.surname !== '') {
                name += contact.surname;
            }

            if (name === '') {
                name = contact.username;
            }

            return name;
        },
        getSeparatorTemplate: function () {
            var template = "<div class=\" px-2 py-1 bg-light text-muted text-center font-size-sm small text-uppercase my-1 rounded-2  \">" +
                "<small>" + l('OtherContacts') + "</small>" +
                "</div>";

            return template;
        },
        addMessage: function (userId, username, name, surname, message, isSent, id) {

            var messageObj = {
                id: id,
                message: message,
                side: isSent ? 1 : 2,
                messageDate: new Date()
            };

            if (contacts.conversations[userId]) {
                contacts.conversations[userId].messages.unshift(messageObj);
            }

            var contactInHtmlList = _wrapper.find('.chat-contact[data-userid=' + userId + ']');

            if (contactInHtmlList.length > 0) {
                $(contactInHtmlList[0]).find('.last-message').html(contacts.shortenMessage(messageObj.message));
                $(contactInHtmlList[0]).find('.last-message').html(contacts.shortenMessage(messageObj.message));
                $(contactInHtmlList[0]).find('.last-message-date').html(dateManager.formatMessageDate(new Date()));
                $(contactInHtmlList[0]).data('is-new-contact', false);

                if (userId !== _wrapper.find('#userId').val()) {
                    var unreadBadge = $(contactInHtmlList[0]).find('.bg-success');
                    if (isSent) {
                        unreadBadge.html('0');
                        unreadBadge.hide();
                    } else {
                        unreadBadge.html(parseInt(unreadBadge.html()) + 1);
                        unreadBadge.show();
                    }
                }

                contacts.moveToTop(userId);
            } else {
                contacts.addNewContact({
                    lastMessage: messageObj.message,
                    lastMessageDate: new Date(),
                    unreadMessageCount: 1,
                    userId: userId,
                    username: username,
                    name: name,
                    surname: surname,
                    lastMessageSide: 2
                });
            }
        },
        addNewContact: function (newContact) {
            var newContactHtml = contacts.getContactTemplate(newContact);

            var currentContactHtml = _wrapper.find('#contact_list').html();
            _wrapper.find('#contact_list').html(newContactHtml + currentContactHtml);
            conversation.bindDeleteConversationEvent();
        },
        moveToTop: function (userId) {

            var contactAsHtmlElement = $(_wrapper.find('.chat-contact[data-userid=' + userId + ']')[0]);

            var previousAll = contactAsHtmlElement.prevAll();

            if (previousAll.length > 0) {
                var top = $(previousAll[previousAll.length - 1]);

                var previous = $(previousAll[0]);

                var moveUp = contactAsHtmlElement.attr('offsetTop') - top.attr('offsetTop');

                var moveDown = (contactAsHtmlElement.offset().top + contactAsHtmlElement.outerHeight()) - (previous.offset().top + previous.outerHeight());

                contactAsHtmlElement.css('position', 'relative');
                previousAll.css('position', 'relative');
                contactAsHtmlElement.animate({ 'top': -moveUp });
                previousAll.animate({ 'top': moveDown }, {
                    complete: function () {
                        contactAsHtmlElement.parent().prepend(contactAsHtmlElement);
                        contactAsHtmlElement.css({ 'position': 'static', 'top': 0 });
                        previousAll.css({ 'position': 'static', 'top': 0 });
                    }
                });
            }
        },
        setContactFilterListener: function () {
            var typingTimer;
            _wrapper.find('#Contacts_Filter').on('keyup',
                function () {
                    clearTimeout(typingTimer);
                    typingTimer = setTimeout(contacts.refresh, 500);
                });
            _wrapper.find('#Contacts_Filter').on('keydown',
                function () {
                    clearTimeout(typingTimer);
                });
            _wrapper.find('#Contacts_Filter').on('search', function () {
                if (_wrapper.find('#Contacts_Filter').val() === '') {
                    contacts.refresh();
                }
            });
        }
    };

    var conversation = {
        init: function () {
            conversation.setSendMessageListeners();
            conversation.setScrollListeners();
            conversation.setTextBoxListeners();
            conversation.getSendOnEnterSetting();
            conversation.setSendOnEnterSettingListener();
        },
        isScrollAtBottom: false,
        loadData: function (userId, unreadMessageCount) {

            if (contacts.conversations[userId]) {
                conversation.display(contacts.conversations[userId], false, unreadMessageCount);
                return;
            }

            _conversationService.getConversation({
                maxResultCount: pageSize, skipCount: 0, targetUserId: userId
            }).then(function (result) {

                contacts.conversations[userId] = result;
                contacts.conversations[userId].page = 1;
                contacts.conversations[userId].HasMoreMessages = result.messages.length < pageSize ? false : true;

                conversation.display(result, false, unreadMessageCount);
            });
        },
        clear: function () {
            $(_wrapper.find('#chat_conversation')[0]).html('');
        },
        loadMoreData: function (userId) {

            var skipCount = contacts.conversations[userId].page * pageSize;

            _conversationService.getConversation({
                maxResultCount: pageSize, skipCount: skipCount, targetUserId: userId
            }).then(function (result) {

                if (result.messages.length === 0) {
                    contacts.conversations[userId].HasMoreMessages = false;
                    return;
                }

                $.merge(contacts.conversations[userId].messages, result.messages);
                contacts.conversations[userId].page = contacts.conversations[userId].page + 1;
                conversation.display(result, true);
            });

        },
        getReceivedMessageTemplate: function (message) {
            var template = "<div message-id=" + message.id + " class=\"media w-75 mw-65 w-lp-auto mb-2\">" +
                //"<img src=\"https://avatars.dicebear.com/v2/human/volorewe.svg\" alt=\"user\" width=\"32\" class=\"rounded-circle d-none d-md-block\">"+
                " <div class=\"media-body position-relative \">" +
                "<div class=\"card bg-light shadow-sm rounded py-1 px-2 py-lg-2 px-lg-3 mb-1\">" +
                "<p class=\"text-small mb-0 \">" + message.message + "</p>" +
                "</div>" +
                "<p class=\"small text-muted position-absolute m-0\" style=\"opacity: 0.5; width: 50px; right: -60px; top: 4px;\">" + dateManager.formatMessageDate(message.messageDate) + "</p>" +
                "</div>" +
                " </div>";

            return template;
        },
        getSentMessageTemplate: function (message) {

            if (message.message == null) {
                message.message = '';
            }

            var deletingTemplate = "";
            if (conversation.isDeletingMessageEnabled(message.messageDate)) {
                deletingTemplate = "<div class=\"dropdown position-absolute top-0 end-0\">" +
                    " <button class=\"btn p-1\" type=\"button\" data-bs-toggle=\"dropdown\" aria-expanded=\"false\">" +
                    "<i class=\"bi bi-chevron-down text-light\" aria-hidden=\"true\"></i>" +
                    "</button>" +
                    "<ul class=\"dropdown-menu\" style=\"\">" +
                    "<li><a class=\"dropdown-item delete-message-btn\" data-id=" + message.id + " href=\"#\">" + l("Delete") + "</a></li>" +
                    "</ul>" +
                    "</div>";
            }

            var template = "<div message-id=" + message.id + " class=\"media w-75 mw-65 w-lp-auto mb-2 ms-auto position-relative\">" +
                "<div class=\"media-body position-relative\">" +
                " <div class=\"bg-primary card mb-1 px-2 px-lg-3 py-1 py-lg-2 rounded shadow-sm\">" +
                " <p class=\"mb-0 message-text text-small text-white\">" + (message.message) + "</p>" +
                " </div>" +
                "<p class=\"left m-0 message-date small text-muted\" style=\"opacity: 0.5; width: 50px; left: -60px; top: 4px;\">" + dateManager.formatMessageDate(message.messageDate) + "</p>" +
                " </div>" +
                deletingTemplate +
                "</div>";

            return template;
        },
        getUnreadMessagesSeparator: function (unreadMessageCount) {
            var unreadMessageCountText = unreadMessageCount > 1
                ? l('YouHave{0}UnreadMessages', unreadMessageCount)
                : l('YouHaveAnUnreadMessage');

            return `<p class=\"text-info small text-center my-2 p-2 border border-info rounded\"> ${unreadMessageCountText} </p>`;

        },
        display: function (data, isExtraData = false, unreadMessageCount = 0) {
            var messagesAsHtml = '';
            for (var i = data.messages.length - 1; i >= 0; i--) {
                if (data.messages[i].side === 1) {
                    messagesAsHtml += conversation.getSentMessageTemplate(data.messages[i]);
                } else {
                    messagesAsHtml += conversation.getReceivedMessageTemplate(data.messages[i]);
                }

                if (!isExtraData && unreadMessageCount > 0 && i === unreadMessageCount) {
                    messagesAsHtml += conversation.getUnreadMessagesSeparator(unreadMessageCount);
                    setTimeout(function () {
                        _wrapper.find('#chat_conversation .unread-message-count-badge-wrapper').remove();
                    }, 5000);
                }
            }

            var userId = _wrapper.find('#Send_Message_Form #userId').val();
            var unreadBadge = $(_wrapper.find('.chat-contact[data-userid=' + userId + ']')[0]).find('.bg-success');

            if (unreadBadge.html().trim() != '0') {
                _conversationService.markConversationAsRead({
                    targetUserId: userId
                });
            }
            unreadBadge.html('0');
            unreadBadge.hide();

            if (isExtraData) {
                var currentMessages = $(_wrapper.find('#chat_conversation')[0]).html();
                $(_wrapper.find('#chat_conversation')[0]).html(messagesAsHtml + currentMessages);

            } else {
                $(_wrapper.find('#chat_conversation')[0]).html(messagesAsHtml);
            }
            conversation.bindDeleteMessageEvent();
            if (!isExtraData) {
                conversation.scrollToBottom();
            }
        },
        setSendMessageListeners: function () {
            _wrapper.find('#Send_Message_Form').submit(function (e) {
                e.preventDefault();
                conversation.sendMessage($(this).serializeFormToObject());
            });

            _wrapper.find('#Send_Message_Form').keydown(function (e) {
                if (e.keyCode == 13 && _wrapper.find('#Send_On_Enter')[0].checked) {
                    e.preventDefault();
                    _wrapper.find("#Send_Message_Button").trigger('click');
                    return false;
                }
            });
        },
        setScrollListeners: function () {
            $(_wrapper.find('#chat_conversation_wrapper')[0]).mCustomScrollbar({
                callbacks: {
                    onScroll: function () {
                        if (this.mcs.draggerTop < 5) {
                            var userId = _wrapper.find('#Send_Message_Form #userId').val();
                            if (contacts.conversations[userId].HasMoreMessages) {
                                conversation.loadMoreData(userId);
                            }
                        }

                        if (this.mcs.topPct === 100) {
                            conversation.isScrollAtBottom = true;
                        } else {
                            conversation.isScrollAtBottom = false;
                        }
                    }
                }
            });
        },
        setTextBoxListeners: function () {
            _wrapper.find('#Send_Message_Form #Chat_Message_Box').on('keyup',
                function () {
                    if (_wrapper.find('#Send_Message_Form #Chat_Message_Box').val() === '' || _wrapper.find('#Send_Message_Form #userId').val() === '') {
                        _wrapper.find('#Send_Message_Form #Send_Message_Button').prop('disabled', true);
                    } else {
                        _wrapper.find('#Send_Message_Form #Send_Message_Button').prop('disabled', false);
                    }
                });
        },
        addMessage: function (message, id) {

            var isScrollAtBottomBeforeMessageAdded = conversation.isScrollAtBottom;

            _wrapper.find('#chat_conversation')
                .append(conversation.getReceivedMessageTemplate({
                    id: id,
                    message: message,
                    messageDate: new Date()
                }));

            if (isScrollAtBottomBeforeMessageAdded) {
                conversation.scrollToBottom();
            }

            _conversationService.markConversationAsRead({
                targetUserId: _wrapper.find('#userId').val()
            });
        },
        sendMessage: function (data) {

            if (!data || data.userId === '' || data.message === '') {
                return;
            }

            _conversationService.sendMessage({
                targetUserId: data.userId, message: data.message
            }).then(function (res) {
                _wrapper.find('#chat_conversation').append(
                    conversation.getSentMessageTemplate({ message: data.message, messageDate: new Date(), id: res.id }));
                _wrapper.find('#Chat_Message_Box').val('');

                if (_wrapper.find('#chat_conversation').html() !== '') {
                    contacts.addMessage(data.userId, null, null, null, data.message, true, res.id);
                } else {
                    var activeContact = _wrapper.find('.chat-contact, .active')[0];

                    contacts.addNewContact({
                        lastMessage: data.message,
                        lastMessageDate: new Date(),
                        unreadMessageCount: 0,
                        userId: data.userId,
                        username: $(activeContact).data('username'),
                        name: $(activeContact).data('conversation-title'),
                        surname: '',
                        lastMessageSide: 1
                    });
                }
                conversation.bindDeleteMessageEvent();
                conversation.scrollToBottom();
                _wrapper.find('#Chat_Message_Box').focus();
                _wrapper.find('#Send_Message_Form #Send_Message_Button').prop('disabled', true);
            });
        },
        deleteConversation: function () {
            var userId = $(this).data('id');
            _conversationService.deleteConversation({
                targetUserId: userId
            }).then((res) => {
                $("div [data-userid=" + userId + "]").remove();
                if (contacts.conversations[userId]) {
                    contacts.conversations[userId] = null;
                }
                if (_wrapper.find('#Send_Message_Form #userId').val() == userId) {
                    $('#EmptyTemplate').show();
                    $('#chat_conversation_wrapper').hide();
                    $('#Send_Message_Form').hide();
                    _wrapper.find('#Conversation_Title').html('');
                    _wrapper.find('#AvatarId').hide();
                    conversation.clear();
                }
            });
        },
        bindDeleteConversationEvent: function () {
            document.querySelectorAll('.delete-conversation-btn').forEach(btn => {
                btn.onclick = conversation.deleteConversation;
            });
        },
        bindDeleteMessageEvent: function () {
            document.querySelectorAll('.delete-message-btn').forEach(btn => {
                btn.onclick = conversation.deleteMessage;
            });
        },
        deleteMessage: function () {
            var messageId = $(this).data('id');
            var userId = _wrapper.find('#Send_Message_Form #userId').val();
            _conversationService.deleteMessage({
                targetUserId: userId,
                messageId: messageId
            }).then((res) => {
                $("div [message-id=" + messageId + "]").remove();
                contacts.refresh(() => conversation.reActiveCurrentContact(userId));
                if (contacts.conversations[userId]) {
                    contacts.conversations[userId].messages = contacts.conversations[userId].messages.filter(m => m.id != messageId);
                }
            });
        },
        reActiveCurrentContact: function (userId) {
            contacts.setActiveContact(document.querySelector(".chat-contact[data-userid='" + userId + "']"));
            contacts.moveToTop(userId);
        },
        getSendOnEnterSetting: function () {
            var checkbox = _wrapper.find("#Send_Message_Form #Send_On_Enter");
            if (abp.setting.getBoolean('Volo.Chat.Messaging.SendMessageOnEnter')) {
                checkbox.prop("checked", true);
            } else {
                checkbox.prop("checked", false);
            }
        },
        setSendOnEnterSettingListener: function () {
            _wrapper.find('#Send_Message_Form #Send_On_Enter').change(function () {
                if (this.checked) {
                    _settingService.setSendOnEnterSetting({ sendOnEnter: true });
                } else {
                    _settingService.setSendOnEnterSetting({ sendOnEnter: false });
                }
            });
        },
        scrollToBottom: function () {
            $(_wrapper.find('#chat_conversation_wrapper')[0]).mCustomScrollbar('scrollTo', 'bottom', {
                scrollInertia: 0
            });
        },
        isDeletingMessageEnabled: function (messageDate) {
            var deletingMessages = abp.setting.get('Volo.Chat.Messaging.DeletingMessages');
            if (deletingMessages === ChatDeletingMessages.enabled) {
                return true;
            }
            if (deletingMessages === ChatDeletingMessages.enabledWithDeletionPeriod) {
                var messageDeletionPeriod = abp.setting.get('Volo.Chat.Messaging.MessageDeletionPeriod');
                return Math.abs((new Date() - new Date(messageDate)) / 1000) < messageDeletionPeriod;
            }

            return false;
        },
        isDeletingConversationEnabled: function () {
            var deletingMessages = abp.setting.get('Volo.Chat.Messaging.DeletingMessages');
            if (deletingMessages !== ChatDeletingMessages.enabled) {
                return false;
            }

            var deletingConversations = abp.setting.get('Volo.Chat.Messaging.DeletingConversations');
            return deletingConversations === ChatDeletingConversations.enabled;
        },
        onDeleteMessage: function (messageId) {
            $("div [message-id=" + messageId + "]").remove();
            var userId = $(".active.chat-contact").data("userid");
            contacts.refresh(() => conversation.reActiveCurrentContact(userId));
            if (contacts.conversations[userId]) {
                contacts.conversations[userId].messages = contacts.conversations[userId].messages.filter(m => m.id != messageId);
            }
        },
        onDeleteConversation: function (userId) {
            $("div [data-userid=" + userId + "]").remove();
            if (_wrapper.find('#Send_Message_Form #userId').val() == userId) {
                $('#EmptyTemplate').show();
                $('#chat_conversation_wrapper').hide();
                $('#Send_Message_Form').hide();
                _wrapper.find('#Conversation_Title').html('');
                _wrapper.find('#AvatarId').hide();
                conversation.clear();
            }
            if (contacts.conversations[userId]) {
                contacts.conversations[userId] = null;
            }
        }
    };

    contacts.init();
    conversation.init();

    $("#StartConversation").click(function () {
        _wrapper.find('#Contacts_Filter').val(' ');
        contacts.refresh();
    });

    $(document).on("ChatMessageReceived", function (event, message) {

        contacts.addMessage(
            message.senderUserId,
            message.senderUsername,
            message.senderName,
            message.senderSurname,
            message.text,
            false,
            message.id
        );

        if (message.senderUserId === _wrapper.find('#userId').val()) {
            conversation.addMessage(message.text, message.id);
        }
    });

    $(document).on("ChatDeleteMessage", function (event, messageId) {
        conversation.onDeleteMessage(messageId)
    });

    $(document).on("ChatDeleteConversation", function (event, userId) {
        conversation.onDeleteConversation(userId)
    });

}(jQuery));
