// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class TemplateDefinitionDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public bool IsLayout { get; set; }

    public string Layout { get; set; }

    public bool IsInlineLocalized { get; set; }

    public string DefaultCultureName { get; set; }
}
