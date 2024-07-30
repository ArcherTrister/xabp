// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

namespace X.Abp.Identity;

public class GetIdentitySecurityLogListInput : PagedAndSortedResultRequestDto
{
    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string ApplicationName { get; set; }

    public string Identity { get; set; }

    public string ActionName { get; set; }

    public string UserName { get; set; }

    public string ClientId { get; set; }

    public string CorrelationId { get; set; }
}
