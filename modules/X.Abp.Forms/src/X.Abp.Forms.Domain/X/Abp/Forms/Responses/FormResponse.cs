// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using JetBrains.Annotations;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

using X.Abp.Forms.Answers;

namespace X.Abp.Forms.Responses;

public class FormResponse : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid? UserId { get; protected set; }

    public virtual Guid FormId { get; protected set; }

    public virtual ICollection<Answer> Answers { get; protected set; }

    public virtual string Email { get; protected set; }

    protected FormResponse()
    {
    }

    public FormResponse(Guid id,
        Guid formId,
        Guid? userId,
        string email = null,
        Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
        UserId = userId;
        FormId = formId;
        SetEmail(email);
        Answers = new Collection<Answer>();
    }

    public virtual void SetEmail(string email)
    {
        Email = Check.Length(email, nameof(email), FormConsts.ResponseConsts.MaxEmailLength);
    }

    public virtual void AddOrUpdateAnswer(Guid questionId, Guid answerId, [CanBeNull] Guid? choiceId, string value)
    {
        var answer = Answers.FirstOrDefault(q => q.Id == answerId);
        if (answer != null)
        {
            answer.UpdateAnswer(newValue: value, choiceId: choiceId);
        }
        else
        {
            Answers.AddIfNotContains(new Answer(
                    id: answerId,
                    formResponseId: Id,
                    questionId: questionId,
                    choiceId: choiceId,
                    value: value));
        }
    }
}
