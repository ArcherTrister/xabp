// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Identity;

public class IdentitySecurityLogDto : EntityDto<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }

    public string ApplicationName { get; set; }

    public string Identity { get; set; }

    public string Action { get; set; }

    public Guid? UserId { get; set; }

    public string UserName { get; set; }

    public string TenantName { get; set; }

    public string ClientId { get; set; }

    public string CorrelationId { get; set; }

    public string ClientIpAddress { get; set; }

    public string BrowserInfo { get; set; }

    public DateTime CreationTime { get; set; }

    public Dictionary<string, object> ExtraProperties { get; set; }
}
