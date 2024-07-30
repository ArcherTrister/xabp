// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.ObjectExtending;

namespace X.Abp.LanguageManagement.Dto;

public class CreateLanguageDto : ExtensibleObject
{
    public string DisplayName { get; set; }

    public string CultureName { get; set; }

    public string UiCultureName { get; set; }

    public string FlagIcon { get; set; }

    public bool IsEnabled { get; set; }

    public CreateLanguageDto()
        : base(false)
    {
    }
}
