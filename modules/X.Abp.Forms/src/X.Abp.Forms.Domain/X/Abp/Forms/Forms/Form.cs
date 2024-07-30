// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using JetBrains.Annotations;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.Forms.Forms;

public class Form : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    public virtual string Title { get; protected set; }

    public virtual string Description { get; protected set; }

    public virtual bool CanEditResponse { get; protected set; }

    public virtual bool IsCollectingEmail { get; protected set; }

    public virtual bool HasLimitOneResponsePerUser { get; protected set; }

    public virtual bool IsAcceptingResponses { get; protected set; }

    public virtual bool IsQuiz { get; protected set; }

    public virtual bool RequiresLogin { get; protected set; }

    protected Form()
    {
    }

    public Form(
        Guid id,
        [NotNull] string title,
        string description = null,
        bool canEditResponse = false,
        bool isCollectingEmail = false,
        bool hasLimitOneResponsePerUser = false,
        bool isAcceptingResponses = true,
        bool isQuiz = false,
        bool requiresLogin = false,
        Guid? tenantId = null)
        : base(id)
    {
        TenantId = tenantId;

        SetTitle(title);
        SetDescription(description);

        SetSettings(
            canEditResponse,
            isCollectingEmail,
            hasLimitOneResponsePerUser,
            isAcceptingResponses,
            isQuiz,
            requiresLogin);
    }

    public virtual void SetTitle(string title)
    {
        Title = Check.NotNullOrWhiteSpace(title, nameof(title), FormConsts.MaxTitleLength);
    }

    public virtual void SetDescription(string description)
    {
        Description = Check.Length(description, nameof(description), FormConsts.MaxDescriptionLength);
    }

    public virtual void SetSettings(
        bool canEditResponse = false,
        bool isCollectingEmail = false,
        bool hasLimitOneResponsePerUser = false,
        bool isAcceptingResponses = true,
        bool isQuiz = false,
        bool requiresLogin = false)
    {
        if (requiresLogin == false)
        {
            hasLimitOneResponsePerUser = false;
        }

        IsQuiz = isQuiz;
        IsCollectingEmail = isCollectingEmail;
        CanEditResponse = canEditResponse;
        IsAcceptingResponses = isAcceptingResponses;
        HasLimitOneResponsePerUser = hasLimitOneResponsePerUser;
        RequiresLogin = requiresLogin;
    }

    public virtual void Undelete()
    {
        IsDeleted = false;
    }
}
