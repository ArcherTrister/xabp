// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Caching;
using Volo.Abp.Localization;

namespace X.Abp.LanguageManagement;

[Serializable]
[CacheName("X.Abp.LanguageList")]
public class LanguageListCacheItem
{
    public List<LanguageInfo> Languages { get; set; }

    public LanguageListCacheItem()
    {
    }

    public LanguageListCacheItem(List<LanguageInfo> languages)
    {
        Languages = languages;
    }
}
