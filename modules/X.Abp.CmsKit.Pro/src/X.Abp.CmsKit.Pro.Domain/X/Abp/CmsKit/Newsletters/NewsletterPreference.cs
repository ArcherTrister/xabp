// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.Newsletters;

public class NewsletterPreference : CreationAuditedEntity<Guid>, IMultiTenant
{
    public virtual Guid NewsletterRecordId { get; protected set; }

    public virtual string Preference { get; protected set; }

    public virtual string Source { get; protected set; }

    public virtual string SourceUrl { get; protected set; }

    public virtual Guid? TenantId { get; protected set; }

    protected NewsletterPreference()
    {
    }

    public NewsletterPreference(
      Guid id,
      Guid newsletterRecordId,
      string preference,
      string source,
      string sourceUrl,
      Guid? tenantId = null)
      : base(id)
    {
        NewsletterRecordId = newsletterRecordId;
        Preference = Check.NotNullOrWhiteSpace(preference, nameof(preference), NewsletterPreferenceConst.MaxPreferenceLength, 0);
        Source = Check.NotNullOrWhiteSpace(source, nameof(source), NewsletterPreferenceConst.MaxSourceLength, 0);
        SourceUrl = Check.NotNullOrWhiteSpace(sourceUrl, nameof(sourceUrl), NewsletterPreferenceConst.MaxSourceUrlLength, 0);
        TenantId = tenantId;
    }
}
