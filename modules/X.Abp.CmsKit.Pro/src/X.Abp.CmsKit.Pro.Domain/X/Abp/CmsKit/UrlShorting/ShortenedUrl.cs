// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.UrlShorting;

public class ShortenedUrl :
BasicAggregateRoot<Guid>,
IMultiTenant,
ICreationAuditedObject,
IHasCreationTime,
IMayHaveCreator
{
    public virtual string Source { get; protected set; }

    public virtual string Target { get; protected set; }

    public virtual Guid? TenantId { get; protected set; }

    public DateTime CreationTime { get; protected set; }

    public Guid? CreatorId { get; protected set; }

    protected ShortenedUrl()
    {
    }

    public ShortenedUrl(Guid id, string source, string target, Guid? tenantId = null)
      : base(id)
    {
        Source = Check.NotNullOrWhiteSpace(source, nameof(source), ShortenedUrlConst.MaxSourceLength, 0);
        SetTarget(target);
        TenantId = tenantId;
    }

    public virtual void SetTarget(string target)
    {
        Target = Check.NotNullOrWhiteSpace(target, nameof(target), ShortenedUrlConst.MaxTargetLength, 0);
    }
}
