// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace X.Abp.Saas.Editions;

public class Edition : FullAuditedAggregateRoot<Guid>, IHasEntityVersion
{
    public virtual string DisplayName { get; protected set; }

    public virtual Guid? PlanId { get; set; }

    public virtual int EntityVersion { get; protected set; }

    protected Edition()
    {
    }

    public Edition(Guid id, string displayName)
        : base(id)
    {
        SetDisplayName(displayName);
    }

    public virtual void SetDisplayName(string displayName)
    {
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), EditionConsts.MaxDisplayNameLength);
    }
}
