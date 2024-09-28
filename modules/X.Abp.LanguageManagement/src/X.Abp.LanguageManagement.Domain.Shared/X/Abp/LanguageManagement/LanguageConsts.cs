// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.LanguageManagement;

public static class LanguageConsts
{
    /// <summary>
    /// Default value: 10
    /// </summary>
    public static int MaxCultureNameLength { get; set; } = 10;

    /// <summary>
    /// Default value: 10
    /// </summary>
    public static int MaxUiCultureNameLength { get; set; } = MaxCultureNameLength;

    /// <summary>
    /// Default value: 32
    /// </summary>
    public static int MaxDisplayNameLength { get; set; } = 32;

    /// <summary>
    /// Default value: 48
    /// </summary>
    public static int MaxFlagIconLength { get; set; } = 48;
}
