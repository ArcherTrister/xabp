// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.Extensions.Options;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Localization;
using Volo.Abp.ObjectMapping;

namespace X.Abp.LanguageManagement;

[Dependency(ReplaceServices = true)]
public class DatabaseLanguageProvider : ILanguageProvider, ITransientDependency
{
    public const string CacheKey = "AllLanguages";

    protected ILanguageRepository LanguageRepository { get; }

    protected IObjectMapper<AbpLanguageManagementDomainModule> ObjectMapper { get; }

    protected IDistributedCache<LanguageListCacheItem> Cache { get; }

    protected AbpLocalizationOptions Options { get; }

    public DatabaseLanguageProvider(
        ILanguageRepository languageRepository,
        IObjectMapper<AbpLanguageManagementDomainModule> objectMapper,
        IDistributedCache<LanguageListCacheItem> cache,
        IOptions<AbpLocalizationOptions> options)
    {
        LanguageRepository = languageRepository;
        ObjectMapper = objectMapper;
        Cache = cache;
        Options = options.Value;
    }

    public virtual async Task<IReadOnlyList<LanguageInfo>> GetLanguagesAsync()
    {
        var cacheItem = await Cache.GetOrAddAsync(CacheKey, async () =>
        {
            var languages = await LanguageRepository.GetListByIsEnabledAsync(isEnabled: true);
            return languages.Count > 0
                ? new LanguageListCacheItem(ObjectMapper.Map<List<Language>, List<LanguageInfo>>(languages))
                : new LanguageListCacheItem(Options.Languages);
        });

        return cacheItem.Languages;
    }
}
