// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.LanguageManagement.Dto;

public class LanguageTextDto
{
    public string ResourceName { get; set; }

    public string CultureName { get; set; }

    public string BaseCultureName { get; set; }

    public string BaseValue { get; set; }

    public string Name { get; set; }

    public string Value { get; set; }
}
