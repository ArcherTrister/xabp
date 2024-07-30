// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.CmsKit.Localization;

namespace X.Abp.CmsKit.Public.Permissions;

public class AbpCmsKitProPublicPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        context.AddGroup(AbpCmsKitProPublicPermissions.GroupName, L("Permission:Public"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<CmsKitResource>(name);
    }
}
