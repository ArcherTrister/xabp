// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.TextTemplating;

namespace X.Abp.TextTemplateManagement.TextTemplates;
public class DynamicTextTemplateDefinitionStoreInMemoryCache :
  IDynamicTextTemplateDefinitionStoreInMemoryCache,
  ISingletonDependency
{
    public string CacheStamp { get; set; }

    protected IDictionary<string, TemplateDefinition> TemplateDefinitions { get; }

    protected ILocalizableStringSerializer LocalizableStringSerializer { get; }

    public SemaphoreSlim SyncSemaphore { get; } = new SemaphoreSlim(1, 1);

    public DateTime? LastCheckTime { get; set; }

    public DynamicTextTemplateDefinitionStoreInMemoryCache(
      ILocalizableStringSerializer localizableStringSerializer)
    {
        LocalizableStringSerializer = localizableStringSerializer;
        TemplateDefinitions = new Dictionary<string, TemplateDefinition>();
    }

    public Task FillAsync(List<TextTemplateDefinitionRecord> templateRecords)
    {
        TemplateDefinitions.Clear();
        foreach (TextTemplateDefinitionRecord templateRecord in templateRecords)
        {
            TemplateDefinition templateDefinition = new TemplateDefinition(templateRecord.Name, templateRecord.LocalizationResourceName, templateRecord.DisplayName != null ? LocalizableStringSerializer.Deserialize(templateRecord.DisplayName) : null, templateRecord.IsLayout, templateRecord.Layout, templateRecord.DefaultCultureName)
            {
                IsInlineLocalized = templateRecord.IsInlineLocalized,
                RenderEngine = templateRecord.RenderEngine
            };
            foreach (KeyValuePair<string, object> extraProperty in (Dictionary<string, object>)templateRecord.ExtraProperties)
            {
                templateDefinition[extraProperty.Key] = extraProperty.Value;
            }

            TemplateDefinitions[templateRecord.Name] = templateDefinition;
        }

        return Task.CompletedTask;
    }

    public TemplateDefinition GetTemplateOrNull(string name)
    {
        return TemplateDefinitions.GetOrDefault(name);
    }

    public IReadOnlyList<TemplateDefinition> GetTemplates()
    {
        return TemplateDefinitions.Values.ToList();
    }
}
