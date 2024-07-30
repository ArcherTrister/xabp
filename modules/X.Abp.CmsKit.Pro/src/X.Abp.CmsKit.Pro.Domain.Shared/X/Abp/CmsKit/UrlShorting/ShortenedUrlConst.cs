// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.CmsKit.UrlShorting;

public static class ShortenedUrlConst
{
    public static int MaxSourceLength { get; set; } = 512;

    public static int MaxTargetLength { get; set; } = 2048;
}
