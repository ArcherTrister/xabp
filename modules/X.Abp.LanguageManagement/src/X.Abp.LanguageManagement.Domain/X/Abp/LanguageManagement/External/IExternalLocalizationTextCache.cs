// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace X.Abp.LanguageManagement.External
{
    public interface IExternalLocalizationTextCache
    {
        Dictionary<string, string> TryGetTextsFromCache(string resourceName, string cultureName);

        Task<Dictionary<string, string>> GetTextsAsync(
            string resourceName,
            string cultureName,
            Func<Task<Dictionary<string, string>>> factory
        );

        Task InvalidateAsync(string resourceName, string cultureName);
    }
}
