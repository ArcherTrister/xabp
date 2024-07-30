// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Caching;
using Volo.Abp.Features;
using Volo.Abp.TextTemplating;

using X.Abp.TextTemplateManagement.Permissions;

namespace X.Abp.TextTemplateManagement.TextTemplates;

[Authorize(AbpTextTemplateManagementPermissions.TextTemplates.Default)]
[RequiresFeature(TextTemplateManagementFeatures.Enable)]
public class TemplateContentAppService :
TextTemplateManagementAppServiceBase,
ITemplateContentAppService
{
    protected ITextTemplateContentRepository TextTemplateContentRepository { get; }

    protected ITemplateContentProvider TemplateContentProvider { get; }

    protected ITemplateDefinitionManager TemplateDefinitionManager { get; }

    protected IDistributedCache<string, TemplateContentCacheKey> Cache { get; }

    public TemplateContentAppService(
      ITextTemplateContentRepository textTemplateContentRepository,
      ITemplateContentProvider templateContentProvider,
      ITemplateDefinitionManager templateDefinitionManager,
      IDistributedCache<string, TemplateContentCacheKey> cache)
    {
        TextTemplateContentRepository = textTemplateContentRepository;
        TemplateContentProvider = templateContentProvider;
        TemplateDefinitionManager = templateDefinitionManager;
        Cache = cache;
    }

    public virtual async Task<TextTemplateContentDto> GetAsync(
      GetTemplateContentInput input)
    {
        var contentOrNullAsync = await TemplateContentProvider.GetContentOrNullAsync(await TemplateDefinitionManager.GetAsync(input.TemplateName), input.CultureName, true, false);
        return new TextTemplateContentDto()
        {
            CultureName = input.CultureName,
            Content = contentOrNullAsync,
            Name = input.TemplateName
        };
    }

    [Authorize(AbpTextTemplateManagementPermissions.TextTemplates.EditContents)]
    public virtual async Task RestoreToDefaultAsync(RestoreTemplateContentInput input)
    {
        var textTemplateContent = await TextTemplateContentRepository.FindAsync((await TemplateDefinitionManager.GetAsync(input.TemplateName)).Name, input.CultureName);
        if (textTemplateContent != null)
        {
            await TextTemplateContentRepository.DeleteAsync(textTemplateContent, false);
            await Cache.RemoveAsync(new TemplateContentCacheKey(input.TemplateName, input.CultureName), null, false);
        }
    }

    [Authorize(AbpTextTemplateManagementPermissions.TextTemplates.EditContents)]
    public virtual async Task<TextTemplateContentDto> UpdateAsync(
      UpdateTemplateContentInput input)
    {
        var templateDefinition = await TemplateDefinitionManager.GetAsync(input.TemplateName);
        var textTemplateContent1 = await TextTemplateContentRepository.FindAsync(input.TemplateName, input.CultureName);
        if (textTemplateContent1 != null)
        {
            await TextTemplateContentRepository.DeleteAsync(textTemplateContent1, true);
        }

        var content = await TemplateContentProvider.GetContentOrNullAsync(templateDefinition, input.CultureName, true, false);
        if (content == input.Content)
        {
            return new TextTemplateContentDto()
            {
                Content = content,
                Name = input.TemplateName,
                CultureName = input.CultureName
            };
        }

        await TextTemplateContentRepository.InsertAsync(new TextTemplateContent(Guid.NewGuid(), input.TemplateName, input.Content, input.CultureName, CurrentTenant.Id), false);
        await Cache.RemoveAsync(new TemplateContentCacheKey(input.TemplateName, input.CultureName));
        return new TextTemplateContentDto()
        {
            Content = input.Content,
            Name = input.TemplateName,
            CultureName = input.CultureName
        };
    }
}
