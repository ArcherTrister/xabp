// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class TemplateContentCacheKey
{
    public string TemplateDefinitionName { get; protected set; }

    public string Culture { get; protected set; }

    public TemplateContentCacheKey(string templateDefinitionName, string culture)
    {
        TemplateDefinitionName = Check.NotNullOrWhiteSpace(templateDefinitionName, nameof(templateDefinitionName));
        Culture = culture;
    }

    public override string ToString()
    {
        return $"{TemplateDefinitionName}_{Culture}";
    }
}
