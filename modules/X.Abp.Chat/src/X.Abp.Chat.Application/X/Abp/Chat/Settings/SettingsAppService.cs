// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Features;
using Volo.Abp.SettingManagement;

using X.Abp.Chat.Features;

namespace X.Abp.Chat.Settings;

[Authorize]
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
}
