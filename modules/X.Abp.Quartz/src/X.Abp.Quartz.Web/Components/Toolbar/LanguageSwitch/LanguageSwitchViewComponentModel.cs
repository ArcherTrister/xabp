// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp.Localization;

namespace X.Abp.Quartz.Web.Components.Toolbar.LanguageSwitch;

public class LanguageSwitchViewComponentModel
{
    public LanguageInfo CurrentLanguage { get; set; }

    public IReadOnlyList<LanguageInfo> OtherLanguages { get; set; }
}
