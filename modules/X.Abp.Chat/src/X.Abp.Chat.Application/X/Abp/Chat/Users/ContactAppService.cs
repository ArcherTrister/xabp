﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Features;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Users;

using X.Abp.Chat.Conversations;
using X.Abp.Chat.Features;
using X.Abp.Chat.Permission;

namespace X.Abp.Chat.Users;

[RequiresFeature(AbpChatFeatures.Enable)]
[Authorize(AbpChatPermissions.Messaging)]
public class ContactAppService : ChatAppServiceBase, IContactAppService
{
    private readonly IChatUserLookupService _chatUserLookupService;
    private readonly IConversationRepository _conversationRepository;
    private readonly IPermissionFinder _permissionFinder;

    public ContactAppService(
        IChatUserLookupService chatUserLookupService,
        IConversationRepository conversationRepository,
        IPermissionFinder permissionFinder)
    {
        _chatUserLookupService = chatUserLookupService;
        _conversationRepository = conversationRepository;
        _permissionFinder = permissionFinder;
    }

    public virtual async Task<List<ChatContactDto>> GetContactsAsync(GetContactsInput input)
    {
        List<ChatContactDto> conversationContacts = (await _conversationRepository.GetListByUserIdAsync(CurrentUser.GetId(), input.Filter)).Select(x => new ChatContactDto()
        {
            UserId = x.TargetUser.Id,
            Name = x.TargetUser?.Name,
            Surname = x.TargetUser?.Surname,
            Username = x.TargetUser?.UserName,
            LastMessageSide = x.Conversation.LastMessageSide,
            LastMessage = x.Conversation.LastMessage,
            LastMessageDate = new DateTime?(x.Conversation.LastMessageDate),
            UnreadMessageCount = x.Conversation.UnreadMessageCount
        }).ToList();

        if (input.IncludeOtherContacts)
        {
            conversationContacts.AddRange((await _chatUserLookupService.SearchAsync(nameof(ChatUser.UserName), input.Filter, AbpChatConsts.OtherContactLimitPerRequest))
                .Where(x => !conversationContacts.Any(c => c.Username == x.UserName) && !(x.Id == CurrentUser.GetId()))
                .Select(x => new ChatContactDto()
            {
                UserId = x.Id,
                Name = x.Name,
                Surname = x.Surname,
                Username = x.UserName
            }));
        }

        List<IsGrantedResponse> source = await _permissionFinder.IsGrantedAsync(conversationContacts.Select(x => new IsGrantedRequest()
        {
            UserId = x.UserId,
            PermissionNames =
            [
              AbpChatPermissions.Messaging
            ]
        }).ToList());

        foreach (ChatContactDto chatContactDto in conversationContacts)
        {
            chatContactDto.HasChatPermission = source.Any(x => x.UserId == chatContactDto.UserId && x.Permissions.All(p => p.Value));
        }

        return conversationContacts;
    }

    public virtual async Task<int> GetTotalUnreadMessageCountAsync()
    {
        return await _conversationRepository.GetTotalUnreadMessageCountAsync(CurrentUser.GetId());
    }
}
