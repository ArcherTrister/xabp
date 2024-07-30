// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

namespace X.Abp.Saas.Dtos;

public class SaasTenantDto : ExtensibleEntityDto<Guid>
{
    public string Name { get; set; }

    public Guid? EditionId { get; set; }

    public DateTime? EditionEndDateUtc { get; set; }

    public string EditionName { get; set; }

    public bool HasDefaultConnectionString { get; set; }

    public TenantActivationState ActivationState { get; set; }

    public DateTime? ActivationEndDate { get; set; }

    public string ConcurrencyStamp { get; set; }
}
