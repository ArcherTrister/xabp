// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.TextTemplateManagement.Permissions;

public class AbpTextTemplateManagementPermissions
{
    public const string GroupName = "TextTemplateManagement";

    public static class TextTemplates
    {
        public const string Default = GroupName + ".TextTemplates";
        public const string EditContents = Default + ".EditContents";
    }
}
