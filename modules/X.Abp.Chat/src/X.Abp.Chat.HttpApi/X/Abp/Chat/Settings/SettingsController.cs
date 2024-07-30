// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;

namespace X.Abp.Chat.Settings;

[RemoteService(Name = AbpChatRemoteServiceConsts.RemoteServiceName)]
[Area(AbpChatRemoteServiceConsts.ModuleName)]
[Route("api/chat/settings")]
public class SettingsController : ChatController, ISettingsAppService
{
    private readonly ISettingsAppService _settingAppService;

    public SettingsController(ISettingsAppService settingAppService)
    {
        _settingAppService = settingAppService;
    }

    [HttpPost]
    [Route("send-on-enter")]
    public Task SetSendOnEnterSettingAsync(SendOnEnterSettingDto input)
    {
        return _settingAppService.SetSendOnEnterSettingAsync(input);
    }
}
