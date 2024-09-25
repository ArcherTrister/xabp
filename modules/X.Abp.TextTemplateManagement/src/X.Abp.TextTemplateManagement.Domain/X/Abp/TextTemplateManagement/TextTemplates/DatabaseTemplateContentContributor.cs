// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.TextTemplating;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public class DatabaseTemplateContentContributor : ITemplateContentContributor, ITransientDependency
{
    protected ITextTemplateContentRepository ContentRepository { get; }

    protected IStaticTemplateDefinitionStore StaticTemplateDefinitionStore { get; }

    protected IDynamicTemplateDefinitionStore DynamicTemplateDefinitionStore { get; }

    protected ITextTemplateDefinitionContentRecordRepository TextTemplateDefinitionContentRecordRepository { get; }

    protected IDistributedCache<string, TemplateContentCacheKey> Cache { get; }

    protected TextTemplateManagementOptions Options { get; }

    public DatabaseTemplateContentContributor(
      ITextTemplateContentRepository contentRepository,
      StaticTemplateDefinitionStore staticTemplateDefinitionStore,
      IDynamicTemplateDefinitionStore dynamicTemplateDefinitionStore,
      ITextTemplateDefinitionContentRecordRepository textTemplateDefinitionContentRecordRepository,
      IDistributedCache<string, TemplateContentCacheKey> cache,
      IOptions<TextTemplateManagementOptions> options)
    {
        ContentRepository = contentRepository;
        StaticTemplateDefinitionStore = staticTemplateDefinitionStore;
        DynamicTemplateDefinitionStore = dynamicTemplateDefinitionStore;
        TextTemplateDefinitionContentRecordRepository = textTemplateDefinitionContentRecordRepository;
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

    protected virtual async Task<string> GetTemplateContentFromDbOrNullAsync(TemplateContentContributorContext context)
    {
        TextTemplateContent textTemplateContent = await ContentRepository.FindAsync(context.TemplateDefinition.Name, context.Culture);
        if (textTemplateContent != null)
        {
            return textTemplateContent.Content;
        }

        TemplateDefinition templateDefinition = await StaticTemplateDefinitionStore.GetOrNullAsync(context.TemplateDefinition.Name);
        if (templateDefinition != null)
        {
            return null;
        }

        templateDefinition = await DynamicTemplateDefinitionStore.GetOrNullAsync(context.TemplateDefinition.Name);
        if (templateDefinition == null)
        {
            return null;
        }

        TextTemplateDefinitionContentRecord definitionContentRecord = await TextTemplateDefinitionContentRecordRepository.FindByDefinitionNameAsync(templateDefinition.Name, context.Culture);
        if (definitionContentRecord != null)
        {
            return definitionContentRecord.FileContent;
        }

        definitionContentRecord = await TextTemplateDefinitionContentRecordRepository.FindByDefinitionNameAsync(templateDefinition.Name, templateDefinition.DefaultCultureName ?? "en");
        if (definitionContentRecord != null)
        {
            return definitionContentRecord.FileContent;
        }

        return (await TextTemplateDefinitionContentRecordRepository.FindByDefinitionNameAsync(templateDefinition.Name) ?? throw new AbpException("Could not find any content for the dynamic template definition: " + templateDefinition.Name + ".")).FileContent;
    }
}
