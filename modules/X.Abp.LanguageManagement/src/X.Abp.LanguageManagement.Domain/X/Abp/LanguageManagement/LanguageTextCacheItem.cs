// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Caching;

namespace X.Abp.LanguageManagement;

[CacheName("X.Abp.LanguageManagement.Texts")]
[Serializable]
public class LanguageTextCacheItem
{
    public Dictionary<string, string> Dictionary { get; set; }

    public LanguageTextCacheItem()
    {
        Dictionary = new Dictionary<string, string>();
    }

    public static string CalculateCacheKey(string resourceName, string cultureName)
    {
        return resourceName + "_" + cultureName;
    }
}
