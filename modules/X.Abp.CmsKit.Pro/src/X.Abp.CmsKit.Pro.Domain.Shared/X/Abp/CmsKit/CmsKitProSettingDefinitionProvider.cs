// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Settings;

namespace X.Abp.CmsKit;

public class CmsKitProSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext value)
    {
        value.Add(new SettingDefinition[]
    {
        new SettingDefinition(CmsKitProSettingNames.Contact.ReceiverEmailAddress, "info@mycompanyname.com", null, null, true, true, false)
    });
    }
}
