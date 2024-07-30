// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

using X.Abp.LanguageManagement.Localization;

namespace X.Abp.LanguageManagement.Permissions;

public class AbpLanguageManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var languageManagementGroup = context.AddGroup(AbpLanguageManagementPermissions.GroupName, L("Permission:LanguageManagement"));

        var textGroup = languageManagementGroup.AddPermission(AbpLanguageManagementPermissions.LanguageTexts.Default, L("Permission:LanguageTexts"))
            .RequireFeatures(LanguageManagementFeatures.Enable);

        textGroup.AddChild(AbpLanguageManagementPermissions.LanguageTexts.Edit, L("Permission:LanguageTextsEdit"))
            .RequireFeatures(LanguageManagementFeatures.Enable);

        var langGroup = languageManagementGroup.AddPermission(AbpLanguageManagementPermissions.Languages.Default, L("Permission:Languages"))
            .RequireFeatures(LanguageManagementFeatures.Enable);

        langGroup.AddChild(AbpLanguageManagementPermissions.Languages.Create, L("Permission:LanguagesCreate"), multiTenancySide: MultiTenancySides.Host)
            .RequireFeatures(LanguageManagementFeatures.Enable);
        langGroup.AddChild(AbpLanguageManagementPermissions.Languages.Edit, L("Permission:LanguagesEdit"), multiTenancySide: MultiTenancySides.Host)
            .RequireFeatures(LanguageManagementFeatures.Enable);
        langGroup.AddChild(AbpLanguageManagementPermissions.Languages.ChangeDefault, L("Permission:LanguagesChangeDefault"))
            .RequireFeatures(LanguageManagementFeatures.Enable);
        langGroup.AddChild(AbpLanguageManagementPermissions.Languages.Delete, L("Permission:LanguagesDelete"), multiTenancySide: MultiTenancySides.Host)
            .RequireFeatures(LanguageManagementFeatures.Enable);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<LanguageManagementResource>(name);
    }
}
