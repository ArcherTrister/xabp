// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.Admin.Newsletters;

[IgnoreMultiTenancy]
[Serializable]
public class ImportInvalidNewslettersCacheItem : IDownloadCacheItem
{
    public List<InvalidImportNewslettersFromFileDto> InvalidNewsletters { get; set; }

    public string Token { get; set; }

    public Guid? TenantId { get; set; }
}
