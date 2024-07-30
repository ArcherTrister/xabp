// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using JetBrains.Annotations;

using Volo.Abp.Domain.Entities;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Forms.Choices;

public class Choice : Entity<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual Guid ChoosableQuestionId { get; private set; }

    public virtual bool IsCorrect { get; private set; }

    public virtual int Index { get; private set; }

    public virtual string Value { get; private set; }

    protected Choice()
    {
    }

    internal Choice(
        Guid id,
        Guid choosableQuestionId,
        int index,
        [NotNull] string value,
        bool isCorrect = false,
        Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
        Index = index;
        ChoosableQuestionId = choosableQuestionId;
        Value = value;
        IsCorrect = isCorrect;
    }

    internal void UpdateIndex(int newIndex)
    {
        Index = newIndex;
    }
}
