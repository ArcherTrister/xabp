// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace X.Abp.Payment.Plans;

public class Plan : FullAuditedAggregateRoot<Guid>
{
    public Plan(Guid id, string name)
      : base(id)
    {
        Name = Check.NotNullOrEmpty(name, nameof(name), PlanConsts.MaxNameLength);
        GatewayPlans = new HashSet<GatewayPlan>();
    }

    public string Name { get; set; }

    public virtual ICollection<GatewayPlan> GatewayPlans { get; protected set; }
}
