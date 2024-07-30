// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Localization;

using Volo.Abp.Localization;

namespace X.Abp.LanguageManagement;

public interface IDynamicResourceLocalizer
{
    LocalizedString GetOrNull(LocalizationResourceBase resource, string cultureName, string name);

    void Fill(LocalizationResourceBase resource, string cultureName, Dictionary<string, LocalizedString> dictionary);

    Task FillAsync(LocalizationResourceBase resource, string cultureName, Dictionary<string, LocalizedString> dictionary);
}
