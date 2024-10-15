// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.Public.PageFeedbacks;

[Serializable]
public class PageFeedbackDto : EntityDto<Guid>, IHasCreationTime, IMultiTenant
{
    public string EntityType { get; set; }

    public string EntityId { get; set; }

    public string Url { get; set; }

    public bool IsUseful { get; set; }

    public string UserNote { get; set; }

    public Guid? TenantId { get; set; }

    public DateTime CreationTime { get; set; }
}
