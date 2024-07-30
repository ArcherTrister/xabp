// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class TextTemplateContent : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; set; }

    public virtual string Name { get; private set; }

    public virtual string CultureName { get; private set; }

    public virtual string Content { get; private set; }

    protected TextTemplateContent()
    {
    }

    public TextTemplateContent(
      Guid id,
      string name,
      string content,
      string cultureName = null,
      Guid? tenantId = null)
      : base(id)
    {
        SetName(name);
        SetCultureName(cultureName);
        SetContent(content);
        TenantId = tenantId;
    }

    public virtual void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), TextTemplateConsts.MaxNameLength, 0);
    }

    public virtual void SetCultureName(string cultureName)
    {
        CultureName = cultureName == null ? null : Check.Length(cultureName, nameof(cultureName), TextTemplateConsts.MaxCultureNameLength, TextTemplateConsts.MinCultureNameLength);
    }

    public virtual void SetContent(string content)
    {
        Content = Check.NotNullOrWhiteSpace(content, nameof(content), TextTemplateConsts.MaxContentLength, 0);
    }
}
