// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Account.Dtos;

public class LinkUserInput : IMultiTenant
{
    public Guid UserId { get; set; }

    public Guid? TenantId { get; set; }

    [Required]
    public string Token { get; set; }
}
