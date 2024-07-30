// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Settings;

namespace X.Abp.Chat.Settings;

public class AbpChatSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(
                AbpChatSettings.Messaging.SendMessageOnEnter,
                true.ToString(),
                isVisibleToClients: true));
    }
}
