// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Payment.Plans;

public class GatewayPlan : Entity, IHasExtraProperties
{
    public GatewayPlan(
      Guid planId,
      string gateway,
      string externalId,
      ExtraPropertyDictionary extraProperties = null)
    {
        PlanId = planId;
        Gateway = Check.NotNullOrEmpty(gateway, nameof(gateway));
        SetExternalId(externalId);
        ExtraProperties = extraProperties ?? new ExtraPropertyDictionary();
    }

    public Guid PlanId { get; protected set; }

    public string Gateway { get; protected set; }

    public string ExternalId { get; protected set; }

    public ExtraPropertyDictionary ExtraProperties { get; }

    public void SetExternalId(string externalId)
    {
        ExternalId = Check.NotNullOrEmpty(externalId, nameof(externalId));
    }

    public override object[] GetKeys()
    {
        return new object[] { PlanId, Gateway };
    }
}
