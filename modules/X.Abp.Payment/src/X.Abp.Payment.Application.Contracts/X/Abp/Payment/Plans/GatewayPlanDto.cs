// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Data;

namespace X.Abp.Payment.Plans;

public class GatewayPlanDto : ExtensibleEntityDto
{
    public Guid PlanId { get; set; }

    public string Gateway { get; set; }

    public string ExternalId { get; set; }

    public GatewayPlanDto()
    {
        ExtraProperties = new ExtraPropertyDictionary();
    }
}
