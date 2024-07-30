// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Saas.Tenants;

public class TenantConnectionString : Entity
{
    public virtual Guid TenantId { get; protected set; }

    public virtual string Name { get; protected set; }

    public virtual string Value { get; protected set; }

    public TenantConnectionString(Guid tenantId, string name, string value)
    {
        TenantId = tenantId;
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConnectionStringConsts.MaxNameLength);
        SetValue(value);
    }

    public virtual void SetValue(string value)
    {
        Value = Check.NotNullOrWhiteSpace(value, nameof(value), TenantConnectionStringConsts.MaxValueLength);
    }

    public override object[] GetKeys()
    {
        return new object[]
        {
            TenantId,
            Name
        };
    }
}
