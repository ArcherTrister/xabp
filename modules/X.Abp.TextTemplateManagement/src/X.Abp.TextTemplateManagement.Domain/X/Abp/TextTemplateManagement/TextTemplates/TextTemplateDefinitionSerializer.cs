// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Localization;
using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.VirtualFiles;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class TextTemplateDefinitionSerializer :
  ITextTemplateDefinitionSerializer,
  ITransientDependency
{
    protected IGuidGenerator GuidGenerator { get; }

    protected ILocalizableStringSerializer LocalizableStringSerializer { get; }

    protected TemplateContentFileProvider TemplateContentFileProvider { get; }

    public TextTemplateDefinitionSerializer(
      IGuidGenerator guidGenerator,
      ILocalizableStringSerializer localizableStringSerializer,
      TemplateContentFileProvider templateContentFileProvider)
    {
        GuidGenerator = guidGenerator;
        LocalizableStringSerializer = localizableStringSerializer;
        TemplateContentFileProvider = templateContentFileProvider;
    }

    public virtual async Task<KeyValuePair<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>>> SerializeAsync(
      TemplateDefinition template)
    {
        using (CultureHelper.Use(CultureInfo.InvariantCulture))
        {
            TextTemplateDefinitionRecord record = new TextTemplateDefinitionRecord(GuidGenerator.Create(), template.Name, LocalizableStringSerializer.Serialize(template.DisplayName), template.IsLayout, template.Layout, template.LocalizationResourceName, template.IsInlineLocalized, template.DefaultCultureName, template.RenderEngine);
            foreach (KeyValuePair<string, object> property in template.Properties)
            {
                record.SetProperty(property.Key, property.Value);
            }

            return new KeyValuePair<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>>(record, value: (await TemplateContentFileProvider.GetFilesAsync(template)).Select(file => new TextTemplateDefinitionContentRecord(GuidGenerator.Create(), record.Id, file.FileName, file.FileContent)).ToList());
        }
    }

    public virtual async Task<Dictionary<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>>> SerializeAsync(IEnumerable<TemplateDefinition> templates)
    {
        Dictionary<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>> list = new Dictionary<TextTemplateDefinitionRecord, List<TextTemplateDefinitionContentRecord>>();
        foreach (TemplateDefinition template in templates)
        {
            var result = await SerializeAsync(template);
            list.Add(result.Key, result.Value);
        }

        return list;
    }
}
