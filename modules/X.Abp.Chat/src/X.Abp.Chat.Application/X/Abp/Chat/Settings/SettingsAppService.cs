// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Features;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;

using X.Abp.Chat.Features;
using X.Abp.Chat.Messages;
using X.Abp.Chat.Permission;

namespace X.Abp.Chat.Settings;

[Authorize(AbpChatPermissions.SettingManagement)]
[RequiresFeature(AbpChatFeatures.Enable)]
public class SettingsAppService : ChatAppServiceBase, ISettingsAppService
{
    protected ISettingManager SettingManager { get; }

    public SettingsAppService(ISettingManager settingManager)
    {
        SettingManager = settingManager;
    }

    public virtual async Task SetSendOnEnterSettingAsync(SendOnEnterSettingDto input)
    {
        await SettingManager.SetForCurrentUserAsync(AbpChatSettings.Messaging.SendMessageOnEnter, input.SendOnEnter.ToString());
    }

    public virtual async Task<ChatSettingsDto> GetAsync()
    {
        return new ChatSettingsDto
        {
            DeletingMessages = Enum.Parse<ChatDeletingMessages>(await SettingProvider.GetOrNullAsync(AbpChatSettings.Messaging.DeletingMessages)),
            MessageDeletionPeriod = await SettingProvider.GetAsync<int>(AbpChatSettings.Messaging.MessageDeletionPeriod),
            DeletingConversations = Enum.Parse<ChatDeletingConversations>(await SettingProvider.GetOrNullAsync(AbpChatSettings.Messaging.DeletingConversations)),
        };
    }

    public virtual async Task UpdateAsync(ChatSettingsDto input)
    {
        if (input != null)
        {
            await SettingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, AbpChatSettings.Messaging.DeletingMessages, input.DeletingMessages.ToString());
            await SettingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, AbpChatSettings.Messaging.MessageDeletionPeriod, input.MessageDeletionPeriod.ToString());
            await SettingManager.SetForTenantOrGlobalAsync(CurrentTenant.Id, AbpChatSettings.Messaging.DeletingConversations, input.DeletingConversations.ToString());
        }
    }
}
