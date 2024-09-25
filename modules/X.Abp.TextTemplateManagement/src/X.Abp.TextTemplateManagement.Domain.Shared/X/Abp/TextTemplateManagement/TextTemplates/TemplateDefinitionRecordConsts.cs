// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.TextTemplateManagement.TextTemplates;
public class TemplateDefinitionRecordConsts
{
    public static int MaxNameLength { get; set; } = 128;

    public static int MaxDisplayNameLength { get; set; } = 256;

    public static int MaxLayoutLength { get; set; } = 256;

    public static int MaxLocalizationResourceNameLength { get; set; } = 256;

    public static int MaxDefaultCultureNameLength { get; set; } = 10;

    public static int MaxRenderEngineLength { get; set; } = 64;
}
