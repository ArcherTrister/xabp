// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Settings;

namespace X.Abp.LeptonTheme.Management.Settings
{
    public class LeptonThemeSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            context.Add(
                new SettingDefinition(LeptonThemeSettingNames.Layout.Boxed, "false", null, null, true),
                new SettingDefinition(LeptonThemeSettingNames.Layout.MenuPlacement, MenuPlacement.Left.ToString(), null, null, true),
                new SettingDefinition(LeptonThemeSettingNames.Layout.MenuStatus, MenuStatus.AlwaysOpened.ToString(), null, null, true),
                new SettingDefinition(LeptonThemeSettingNames.Style, LeptonStyle.Style1.ToString(), null, null, true),
                new SettingDefinition(LeptonThemeSettingNames.PublicLayoutStyle, LeptonStyle.Style1.ToString(), null, null, true));
        }
    }
}
