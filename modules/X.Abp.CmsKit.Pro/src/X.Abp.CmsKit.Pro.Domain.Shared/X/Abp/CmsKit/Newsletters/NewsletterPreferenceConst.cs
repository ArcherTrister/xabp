// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using X.Abp.CmsKit.Entities;

namespace X.Abp.CmsKit.Newsletters;

public static class NewsletterPreferenceConst
{
    public static int MaxPreferenceLength { get; set; } = CmsEntityConsts.MaxPreferenceLength;

    public static int MaxSourceLength { get; set; } = CmsEntityConsts.MaxSourceLength;

    public static int MaxSourceUrlLength { get; set; } = 256;
}
