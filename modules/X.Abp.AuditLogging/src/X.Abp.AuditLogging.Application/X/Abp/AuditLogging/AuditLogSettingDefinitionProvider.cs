// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AuditLogging.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Settings;

namespace X.Abp.AuditLogging;
public class AuditLogSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        context.Add(
            new SettingDefinition(AuditLogSettingNames.IsPeriodicDeleterEnabled, "false", L("DisplayName:IsPeriodicDeleterEnabled"), L("Description:IsPeriodicDeleterEnabled"), isInherited: false)
            .WithProviders(GlobalSettingValueProvider.ProviderName, TenantSettingValueProvider.ProviderName),
            new SettingDefinition(AuditLogSettingNames.IsExpiredDeleterEnabled, "false", L("DisplayName:IsExpiredDeleterEnabled"), L("Description:IsExpiredDeleterEnabled"), isInherited: false),
            new SettingDefinition(AuditLogSettingNames.ExpiredDeleterPeriod, "0", L("DisplayName:ExpiredDeleterPeriod"), L("Description:ExpiredDeleterPeriod"), isInherited: false));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AuditLoggingResource>(name);
    }
}
