// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace X.Abp.Saas.Tenants;

public class Tenant : FullAuditedAggregateRoot<Guid>, IHasEntityVersion
{
    public virtual string Name { get; protected set; }

    public virtual string NormalizedName { get; protected set; }

    public virtual Guid? EditionId { get; set; }

    public virtual DateTime? EditionEndDateUtc { get; set; }

    public virtual List<TenantConnectionString> ConnectionStrings { get; protected set; }

    public virtual TenantActivationState ActivationState { get; protected set; }

    public virtual DateTime? ActivationEndDate { get; protected set; }

    public virtual int EntityVersion { get; protected set; }

    protected Tenant()
    {
    }

    protected internal Tenant(Guid id, string name, string normalizedName, Guid? editionId = null)
    {
        Id = id;
        SetName(name);
        SetNormalizedName(normalizedName);
        EditionId = editionId;
        ConnectionStrings = new List<TenantConnectionString>();
    }

    protected internal virtual void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), TenantConsts.MaxNameLength);
    }

    protected internal virtual void SetNormalizedName(string normalizedName)
    {
        NormalizedName = Check.NotNullOrWhiteSpace(normalizedName, nameof(normalizedName), TenantConsts.MaxNameLength);
    }

    public virtual string FindDefaultConnectionString()
    {
        return FindConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
    }

    public virtual string FindConnectionString(string name)
    {
        return ConnectionStrings.FirstOrDefault(c => c.Name == name)?.Value;
    }

    public virtual void SetDefaultConnectionString(string connectionString)
    {
        SetConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName, connectionString);
    }

    public virtual void SetConnectionString(string name, string connectionString)
    {
        var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);
        if (tenantConnectionString != null)
        {
            if (tenantConnectionString.Value != connectionString)
            {
                tenantConnectionString.SetValue(connectionString);
                return;
            }
        }
        else
        {
            ConnectionStrings.Add(new TenantConnectionString(Id, name, connectionString));
        }
    }

    public virtual void RemoveDefaultConnectionString()
    {
        RemoveConnectionString(Volo.Abp.Data.ConnectionStrings.DefaultConnectionStringName);
    }

    public virtual void RemoveConnectionString(string name)
    {
        var tenantConnectionString = ConnectionStrings.FirstOrDefault(x => x.Name == name);
        if (tenantConnectionString != null)
        {
            ConnectionStrings.Remove(tenantConnectionString);
        }
    }

    public virtual void SetActivationState(TenantActivationState activationState)
    {
        ActivationState = activationState;
        if (ActivationState != TenantActivationState.ActiveWithLimitedTime)
        {
            ActivationEndDate = null;
        }
    }

    public virtual void SetActivationEndDate(DateTime? activationEndDate)
    {
        if (ActivationState == TenantActivationState.ActiveWithLimitedTime)
        {
            ActivationEndDate = activationEndDate;
            return;
        }
    }

    public virtual Guid? GetActiveEditionId()
    {
        return EditionEndDateUtc == null ? EditionId : EditionEndDateUtc >= DateTime.UtcNow ? EditionId : null;
    }

    public virtual void SetEdition([CanBeNull] Guid? editionId)
    {
        EditionId = editionId;
    }
}
