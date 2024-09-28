// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.LanguageManagement;
public static class LocalizationTextRecordConsts
{
    /// <summary>
    /// Default value: 10
    /// </summary>
    public static int MaxCultureNameLength { get; set; } = LanguageConsts.MaxCultureNameLength;

    /// <summary>
    /// Default value: 128
    /// </summary>
    public static int MaxResourceNameLength { get; set; } = 128;

    /// <summary>
    /// Default value: 1024 * 1024
    /// </summary>
    public static int MaxValueLength { get; set; } = 1048576;
}
