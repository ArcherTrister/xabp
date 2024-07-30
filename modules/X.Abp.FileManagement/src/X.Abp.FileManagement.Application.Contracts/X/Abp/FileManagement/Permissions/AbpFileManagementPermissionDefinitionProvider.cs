// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Features;
using Volo.Abp.Localization;

using X.Abp.FileManagement.Localization;

namespace X.Abp.FileManagement.Permissions;

public class AbpFileManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var fileManagementGroup = context.AddGroup(AbpFileManagementPermissions.GroupName, L("Permission:FileManagement"));

        var directoryPermission = fileManagementGroup.AddPermission(AbpFileManagementPermissions.DirectoryDescriptor.Default, L("Permission:FileManagement:Directory"))
            .RequireFeatures(FileManagementFeatures.Enable);
        directoryPermission.AddChild(AbpFileManagementPermissions.DirectoryDescriptor.Create, L("Permission:FileManagement:Directory:Create"))
            .RequireFeatures(FileManagementFeatures.Enable);
        directoryPermission.AddChild(AbpFileManagementPermissions.DirectoryDescriptor.Update, L("Permission:FileManagement:Directory:Update"))
            .RequireFeatures(FileManagementFeatures.Enable);
        directoryPermission.AddChild(AbpFileManagementPermissions.DirectoryDescriptor.Delete, L("Permission:FileManagement:Directory:Delete"))
            .RequireFeatures(FileManagementFeatures.Enable);

        var filePermission = fileManagementGroup.AddPermission(AbpFileManagementPermissions.FileDescriptor.Default, L("Permission:FileManagement:File"))
            .RequireFeatures(FileManagementFeatures.Enable);
        filePermission.AddChild(AbpFileManagementPermissions.FileDescriptor.Create, L("Permission:FileManagement:File:Create"))
            .RequireFeatures(FileManagementFeatures.Enable);
        filePermission.AddChild(AbpFileManagementPermissions.FileDescriptor.Update, L("Permission:FileManagement:File:Update"))
            .RequireFeatures(FileManagementFeatures.Enable);
        filePermission.AddChild(AbpFileManagementPermissions.FileDescriptor.Delete, L("Permission:FileManagement:File:Delete"))
            .RequireFeatures(FileManagementFeatures.Enable);
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FileManagementResource>(name);
    }
}
