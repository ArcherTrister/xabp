// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Account.Dtos;

public class LinkUserDto
{
    public virtual Guid TargetUserId { get; set; }

    public virtual string TargetUserName { get; set; }

    public virtual Guid? TargetTenantId { get; set; }

    public virtual string TargetTenantName { get; set; }

    public virtual bool DirectlyLinked { get; set; }
}
