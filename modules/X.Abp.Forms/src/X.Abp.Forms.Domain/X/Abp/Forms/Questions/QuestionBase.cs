// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

using X.Abp.Forms.Questions.ChoosableItems;

namespace X.Abp.Forms.Questions;

public abstract class QuestionBase : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual Guid FormId { get; protected set; }

    public virtual int Index { get; private set; }

    public virtual string Title { get; private set; }

    public virtual string Description { get; private set; }

    protected QuestionBase()
    {
    }

    internal QuestionBase(Guid id, Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;
    }

    public virtual Guid GetId()
    {
        return Id;
    }

    public virtual int GetIndex()
    {
        return Index;
    }

    public virtual QuestionBase SetIndex(int index)
    {
        Index = index;
        return this;
    }

    public virtual string GetTitle()
    {
        return Title;
    }

    public virtual QuestionBase SetTitle(string title)
    {
        Title = title;
        return this;
    }

    public virtual string GetDescription()
    {
        return Description;
    }

    public virtual QuestionBase SetDescription(string description)
    {
        Description = description;
        return this;
    }

    public virtual QuestionTypes GetQuestionType()
    {
        var questionTypeAttribute = (QuestionTypeAttribute)Attribute.GetCustomAttribute(GetType(), typeof(QuestionTypeAttribute));

        return questionTypeAttribute == null
            ? throw new AbpException($"QuestionTypeAttribute is not set for {GetType()}")
            : questionTypeAttribute.QuestionType;
    }

    public virtual QuestionBase SetFormId(Guid formId)
    {
        FormId = formId;
        return this;
    }

    public virtual void SetRequired(bool isRequired)
    {
        if (this is IRequired)
        {
            ((IRequired)this).IsRequired = isRequired;
        }
    }

    public virtual void SetOtherOption(bool hasOtherOption)
    {
        if (this is IHasOtherOption)
        {
            ((IHasOtherOption)this).HasOtherOption = hasOtherOption;
        }
    }
}
