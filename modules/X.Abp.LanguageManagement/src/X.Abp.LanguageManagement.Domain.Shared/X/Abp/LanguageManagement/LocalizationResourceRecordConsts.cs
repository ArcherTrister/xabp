// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.LanguageManagement;
public static class LocalizationResourceRecordConsts
{
    /// <summary>
    /// Default value: 128
    /// </summary>
    public static int MaxNameLength { get; set; } = 128;

    /// <summary>
    /// Default value: 1280 (10 x <see cref="P:Volo.Abp.LanguageManagement.LocalizationResourceRecordConsts.MaxNameLength" />)
    /// </summary>
    public static int MaxBaseResourcesLength { get; set; } = MaxNameLength * 10;

    /// <summary>
    /// Default value: 10 (<see cref="P:Volo.Abp.LanguageManagement.LanguageConsts.MaxCultureNameLength" />)
    /// </summary>
    public static int MaxDefaultCultureLength { get; set; } = LanguageConsts.MaxCultureNameLength;

    public static int MaxSupportedCulturesLength { get; set; } = LanguageConsts.MaxCultureNameLength * 64;
}
