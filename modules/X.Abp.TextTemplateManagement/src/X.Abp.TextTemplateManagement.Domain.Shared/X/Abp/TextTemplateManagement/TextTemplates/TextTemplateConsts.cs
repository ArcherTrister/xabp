// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class TextTemplateConsts
{
    public static int MaxNameLength { get; set; } = 128;

    public static int MinCultureNameLength { get; set; } = 1;

    public static int MaxCultureNameLength { get; set; } = 10;

    public static int MaxContentLength { get; set; } = 65535;
}
