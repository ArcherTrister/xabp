// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

using X.Abp.TextTemplateManagement.Features;
using X.Abp.TextTemplateManagement.Localization;

namespace X.Abp.TextTemplateManagement.Permissions;

public class AbpTextTemplateManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var textTemplateManagementGroup = context.AddGroup(AbpTextTemplateManagementPermissions.GroupName, L("Permission:TextTemplateManagement"));
        textTemplateManagementGroup
            .AddPermission(AbpTextTemplateManagementPermissions.TextTemplates.Default, L("Permission:TextTemplates"), MultiTenancySides.Both).RequireFeatures(TextTemplateManagementFeatures.Enable)
            .AddChild(AbpTextTemplateManagementPermissions.TextTemplates.EditContents, L("Permission:EditContents"), MultiTenancySides.Both).RequireFeatures(TextTemplateManagementFeatures.Enable);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<TextTemplateManagementResource>(name);
    }
}
