// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.LanguageManagement;

public class LanguageText : AuditedEntity<Guid>, IAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual string ResourceName { get; set; }

    public virtual string CultureName { get; set; }

    public virtual string Name { get; set; }

    public virtual string Value { get; set; }

    protected LanguageText()
    {
    }

    public LanguageText(
        Guid id,
        string resourceName,
        string cultureName,
        string name,
        string value = null,
        Guid? tenantId = null)
    {
        Check.NotNullOrWhiteSpace(resourceName, nameof(resourceName));
        Check.NotNullOrWhiteSpace(cultureName, nameof(cultureName));
        Check.NotNullOrWhiteSpace(name, nameof(name));

        Id = id;
        ResourceName = resourceName;
        CultureName = cultureName;
        Name = name;
        Value = value;
        TenantId = tenantId;
    }
}
