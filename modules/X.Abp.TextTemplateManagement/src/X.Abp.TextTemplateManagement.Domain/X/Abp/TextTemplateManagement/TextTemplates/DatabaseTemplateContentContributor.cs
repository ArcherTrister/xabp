// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TextTemplating;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class DatabaseTemplateContentContributor : ITemplateContentContributor, ITransientDependency
{
    protected ITextTemplateContentRepository ContentRepository { get; }

    protected IDistributedCache<string, TemplateContentCacheKey> Cache { get; }

    protected TextTemplateManagementOptions Options { get; }

    public DatabaseTemplateContentContributor(
      ITextTemplateContentRepository contentRepository,
      IDistributedCache<string, TemplateContentCacheKey> cache,
      IOptions<TextTemplateManagementOptions> options)
    {
        ContentRepository = contentRepository;
        Cache = cache;
        Options = options.Value;
    }

    public virtual async Task<string> GetOrNullAsync(TemplateContentContributorContext context)
    {
        return await Cache.GetOrAddAsync(
            new TemplateContentCacheKey(context.TemplateDefinition.Name, context.Culture),
            async () => await GetTemplateContentFromDbOrNullAsync(context),
            () => new DistributedCacheEntryOptions()
            {
                SlidingExpiration = Options.MinimumCacheDuration
            });
    }

    protected virtual async Task<string> GetTemplateContentFromDbOrNullAsync(
      TemplateContentContributorContext context)
    {
        return (await ContentRepository.FindAsync(context.TemplateDefinition.Name, context.Culture))?.Content;
    }
}
