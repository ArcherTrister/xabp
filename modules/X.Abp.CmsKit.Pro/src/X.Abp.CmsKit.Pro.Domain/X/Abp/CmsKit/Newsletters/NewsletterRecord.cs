// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.Newsletters;

public class NewsletterRecord : CreationAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual string EmailAddress { get; protected set; }

    public virtual ICollection<NewsletterPreference> Preferences { get; protected set; }

    public virtual Guid? TenantId { get; protected set; }

    protected NewsletterRecord()
    {
    }

    public NewsletterRecord(Guid id, string emailAddress, Guid? tenantId = null)
      : base(id)
    {
        SetEmailAddress(emailAddress);
        Preferences = new Collection<NewsletterPreference>();
        TenantId = tenantId;
    }

    public NewsletterRecord SetEmailAddress(string emailAddress)
    {
        EmailAddress = Check.NotNullOrWhiteSpace(emailAddress, nameof(emailAddress), NewsletterRecordConst.MaxEmailAddressLength, 0);
        return this;
    }

    public NewsletterRecord AddPreferences(NewsletterPreference preference)
    {
        Preferences.AddIfNotContains(preference);
        return this;
    }

    public NewsletterRecord RemovePreference(Guid id)
    {
        Preferences.Remove(Preferences.First(x => x.Id == id));
        return this;
    }
}
