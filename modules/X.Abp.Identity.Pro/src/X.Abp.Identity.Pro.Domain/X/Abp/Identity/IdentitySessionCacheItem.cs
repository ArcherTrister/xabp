// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Identity;

[Serializable]
public class IdentitySessionCacheItem
{
    public Guid Id { get; set; }

    public virtual string SessionId { get; set; }

    public virtual string IpAddress { get; set; }

    public virtual DateTime? CacheLastAccessed { get; set; }

    public virtual int HitCount { get; set; }
}
