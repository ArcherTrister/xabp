// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.Polls;

public class PollOption : Entity<Guid>, IMultiTenant
{
    public virtual Guid PollId { get; protected set; }

    public virtual string Text { get; protected set; }

    public virtual int VoteCount { get; protected set; }

    public virtual Guid? TenantId { get; protected set; }

    public virtual int Order { get; protected set; }

    protected PollOption()
    {
    }

    internal PollOption(Guid id, string text, int order, Guid pollId, Guid? tenantId)
      : base(id)
    {
        SetText(text);
        SetOrder(order);
        PollId = pollId;
        Order = order;
        TenantId = tenantId;
    }

    public virtual void SetText(string text)
    {
        Text = Check.NotNullOrEmpty(text, nameof(text), PollConst.MaxTextLength, 0);
    }

    public virtual void SetOrder(int order)
    {
        Order = order;
    }

    public virtual void Increase()
    {
        ++VoteCount;
    }
}
