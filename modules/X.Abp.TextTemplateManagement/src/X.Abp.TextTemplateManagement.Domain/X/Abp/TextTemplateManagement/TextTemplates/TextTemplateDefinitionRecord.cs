// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class TextTemplateDefinitionRecord : BasicAggregateRoot<Guid>, IHasExtraProperties
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public bool IsLayout { get; set; }

    public string Layout { get; set; }

    public string LocalizationResourceName { get; set; }

    public bool IsInlineLocalized { get; set; }

    public string DefaultCultureName { get; set; }

    public string RenderEngine { get; set; }

    public ExtraPropertyDictionary ExtraProperties { get; protected set; }

    public TextTemplateDefinitionRecord()
    {
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public TextTemplateDefinitionRecord(
      Guid id,
      string name,
      string displayName,
      bool isLayout,
      string layout,
      string localizationResourceName,
      bool isInlineLocalized,
      string defaultCultureName,
      string renderEngine)
      : base(id)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), TemplateDefinitionRecordConsts.MaxNameLength);
        DisplayName = Check.NotNullOrWhiteSpace(displayName, nameof(displayName), TemplateDefinitionRecordConsts.MaxDisplayNameLength);
        IsLayout = isLayout;
        Layout = Check.Length(layout, nameof(layout), TemplateDefinitionRecordConsts.MaxLayoutLength);
        LocalizationResourceName = Check.Length(localizationResourceName, nameof(localizationResourceName), TemplateDefinitionRecordConsts.MaxLocalizationResourceNameLength);
        IsInlineLocalized = isInlineLocalized;
        DefaultCultureName = Check.Length(defaultCultureName, nameof(defaultCultureName), TemplateDefinitionRecordConsts.MaxDefaultCultureNameLength);
        RenderEngine = Check.Length(renderEngine, nameof(renderEngine), TemplateDefinitionRecordConsts.MaxRenderEngineLength);
        ExtraProperties = new ExtraPropertyDictionary();
        this.SetDefaultsForExtraProperties();
    }

    public bool HasSameData(TextTemplateDefinitionRecord otherRecord)
    {
        return !(Name != otherRecord.Name) && !(DisplayName != otherRecord.DisplayName) && IsLayout == otherRecord.IsLayout && !(Layout != otherRecord.Layout) && !(LocalizationResourceName != otherRecord.LocalizationResourceName) && IsInlineLocalized == otherRecord.IsInlineLocalized && !(DefaultCultureName != otherRecord.DefaultCultureName) && !(RenderEngine != otherRecord.RenderEngine) && this.HasSameExtraProperties(otherRecord);
    }

    public void Patch(TextTemplateDefinitionRecord otherRecord)
    {
        if (Name != otherRecord.Name)
        {
            Name = otherRecord.Name;
        }

        if (DisplayName != otherRecord.DisplayName)
        {
            DisplayName = otherRecord.DisplayName;
        }

        if (IsLayout != otherRecord.IsLayout)
        {
            IsLayout = otherRecord.IsLayout;
        }

        if (Layout != otherRecord.Layout)
        {
            Layout = otherRecord.Layout;
        }

        if (LocalizationResourceName != otherRecord.LocalizationResourceName)
        {
            LocalizationResourceName = otherRecord.LocalizationResourceName;
        }

        if (IsInlineLocalized != otherRecord.IsInlineLocalized)
        {
            IsInlineLocalized = otherRecord.IsInlineLocalized;
        }

        if (DefaultCultureName != otherRecord.DefaultCultureName)
        {
            DefaultCultureName = otherRecord.DefaultCultureName;
        }

        if (RenderEngine != otherRecord.RenderEngine)
        {
            RenderEngine = otherRecord.RenderEngine;
        }

        if (this.HasSameExtraProperties(otherRecord))
        {
            return;
        }

        ExtraProperties.Clear();
        foreach (KeyValuePair<string, object> extraProperty in (Dictionary<string, object>)otherRecord.ExtraProperties)
        {
            ExtraProperties.Add(extraProperty.Key, extraProperty.Value);
        }
    }
}
