// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

namespace X.Abp.Saas.Dtos;

public class GetTenantsInput : PagedAndSortedResultRequestDto
{
    public string Filter { get; set; }

    public bool GetEditionNames { get; set; } = true;

    public Guid? EditionId { get; set; }

    public DateTime? ExpirationDateMin { get; set; }

    public DateTime? ExpirationDateMax { get; set; }

    public TenantActivationState? ActivationState { get; set; }
}
