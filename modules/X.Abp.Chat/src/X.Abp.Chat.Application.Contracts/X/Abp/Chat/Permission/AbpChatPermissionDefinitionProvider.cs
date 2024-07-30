// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;

using X.Abp.Chat.Features;
using X.Abp.Chat.Localization;

namespace X.Abp.Chat.Permission;

public class AbpChatPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AbpChatPermissions.GroupName, L("Permission:Chat"));
        group.AddPermission(AbpChatPermissions.Messaging, L("Permission:Messaging"))
            .RequireFeatures(AbpChatFeatures.Enable);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AbpChatResource>(name);
    }
}
