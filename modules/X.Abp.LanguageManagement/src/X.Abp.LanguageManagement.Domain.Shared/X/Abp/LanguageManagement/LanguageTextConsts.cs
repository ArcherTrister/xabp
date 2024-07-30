// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.LanguageManagement;

public static class LanguageTextConsts
{
    public static int MaxResourceNameLength { get; }

    public static int MaxKeyNameLength { get; }

    public static int MaxValueLength { get; }

    public static int MaxCultureNameLength { get; }

    static LanguageTextConsts()
    {
        MaxResourceNameLength = 128;
        MaxKeyNameLength = 512;
        MaxValueLength = 67108864;
        MaxCultureNameLength = LanguageConsts.MaxCultureNameLength;
    }
}
