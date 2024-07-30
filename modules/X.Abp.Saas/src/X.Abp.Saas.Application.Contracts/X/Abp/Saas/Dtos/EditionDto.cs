// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Saas.Dtos;

public class EditionDto : ExtensibleEntityDto<Guid>, IHasConcurrencyStamp
{
    public string DisplayName { get; set; }

    public Guid? PlanId { get; set; }

    public string PlanName { get; set; }

    public string ConcurrencyStamp { get; set; }
}
