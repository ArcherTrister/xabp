// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Forms.Forms;

public class FormDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp, IMultiTenant
{
    public string Title { get; set; }

    public string Description { get; set; }

    public Guid? TenantId { get; set; }

    public bool CanEditResponse { get; set; }

    public bool IsCollectingEmail { get; set; }

    public bool RequiresLogin { get; set; }

    public bool HasLimitOneResponsePerUser { get; set; }

    public bool IsAcceptingResponses { get; set; }

    public bool IsQuiz { get; set; }

    public string ConcurrencyStamp { get; set; }
}
