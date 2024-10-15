// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using X.Abp.CmsKit.Entities;

namespace X.Abp.CmsKit.PageFeedbacks;

public static class PageFeedbackConst
{
    public const string EmailAddressesSeparator = ",";
    public const string DefaultSettingEntityType = null;

    public static int MaxEntityTypeLength { get; set; } = CmsEntityConsts.MaxEntityTypeLength;

    public static int MaxEntityIdLength { get; set; } = CmsEntityConsts.MaxEntityIdLength;

    public static int MaxUrlLength { get; set; } = 256;

    public static int MaxUserNoteLength { get; set; } = 1024;

    public static int MaxAdminNoteLength { get; set; } = 1024;

    public static int MaxEmailAddressesLength { get; set; } = 1024;
}
