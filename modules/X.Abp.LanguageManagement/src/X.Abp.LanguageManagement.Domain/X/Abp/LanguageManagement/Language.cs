﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using JetBrains.Annotations;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Localization;

namespace X.Abp.LanguageManagement;

public class Language : FullAuditedAggregateRoot<Guid>, ILanguageInfo
{
    [NotNull]
    public virtual string CultureName { get; protected set; }

    [NotNull]
    public virtual string UiCultureName { get; protected set; }

    [NotNull]
    public virtual string DisplayName { get; protected set; }

    [CanBeNull]
    public virtual string FlagIcon { get; set; }

    public virtual bool IsEnabled { get; set; }

    protected Language()
    {
    }

    public Language(
        Guid id,
        string cultureName,
        string uiCultureName = null,
        string displayName = null,
        string flagIcon = null,
        bool isEnabled = true)
    {
        Id = id;
        ChangeCultureInternal(cultureName, uiCultureName, displayName);
        FlagIcon = flagIcon;
        IsEnabled = isEnabled;
    }

    public virtual void ChangeCulture(string cultureName, string uiCultureName = null, string displayName = null)
    {
        ChangeCultureInternal(cultureName, uiCultureName, displayName);
    }

    protected virtual void ChangeCultureInternal(string cultureName, string uiCultureName, string displayName)
    {
        CultureName = Check.NotNullOrWhiteSpace(cultureName, nameof(cultureName));

        UiCultureName = !uiCultureName.IsNullOrWhiteSpace()
            ? uiCultureName
            : cultureName;

        DisplayName = !displayName.IsNullOrWhiteSpace()
            ? displayName
            : cultureName;
    }

    public virtual void SetDisplayName(string displayName)
    {
        DisplayName = displayName;
    }
}
