// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Identity;

public class IdentitySessionDto
{
    public virtual Guid Id { get; set; }

    public virtual string SessionId { get; set; }

    public virtual bool IsCurrent { get; set; }

    public virtual string Device { get; set; }

    public virtual string DeviceInfo { get; set; }

    public virtual Guid? TenantId { get; set; }

    public virtual string TenantName { get; set; }

    public virtual Guid UserId { get; set; }

    public virtual string UserName { get; set; }

    public virtual string ClientId { get; set; }

    public virtual string[] IpAddresses { get; set; }

    public virtual DateTime SignedIn { get; set; }

    public virtual DateTime? LastAccessed { get; set; }
}
