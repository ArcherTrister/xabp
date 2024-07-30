// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using JetBrains.Annotations;

using Microsoft.Extensions.Localization;

namespace X.Abp.Account.Admin.Web.Pages.Account.Components.AccountSettingGroup;

public static class ExternalLoginProviderLocalizationHelper
{
    public static string Localize(
        [CanBeNull] IStringLocalizer localizer,
        [NotNull] string key,
        [NotNull] string defaultValue)
    {
        if (localizer == null)
        {
            return defaultValue;
        }

        var result = localizer[key];
        return result.ResourceNotFound ? defaultValue : result.Value;
    }
}
