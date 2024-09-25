// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Features;
using Volo.Abp.Localization;
using Volo.Abp.TextTemplating;

using X.Abp.TextTemplateManagement.Features;
using X.Abp.TextTemplateManagement.Permissions;

namespace X.Abp.TextTemplateManagement.TextTemplates;

[Authorize(AbpTextTemplateManagementPermissions.TextTemplates.Default)]
[RequiresFeature(TextTemplateManagementFeatures.Enable)]
public class TemplateDefinitionAppService :
  TextTemplateManagementAppServiceBase,
  ITemplateDefinitionAppService
{
    protected ITemplateDefinitionManager TemplateDefinitionManager { get; }

    public TemplateDefinitionAppService(
      ITemplateDefinitionManager templateDefinitionManager)
    {
        TemplateDefinitionManager = templateDefinitionManager;
    }

    public virtual async Task<PagedResultDto<TemplateDefinitionDto>> GetListAsync(
      GetTemplateDefinitionListInput input)
    {
        var templateDefinitions = await TemplateDefinitionManager.GetAllAsync();
        var source = new List<TemplateDefinitionDto>();
        foreach (var templateDefinition in templateDefinitions)
        {
            var templateDefinitionDto = ObjectMapper.Map<TemplateDefinition, TemplateDefinitionDto>(templateDefinition);
            templateDefinitionDto.DisplayName = templateDefinition.GetLocalizedDisplayName(StringLocalizerFactory);
            source.Add(templateDefinitionDto);
        }

        if (!string.IsNullOrWhiteSpace(input.FilterText))
        {
            input.FilterText = input.FilterText.ToUpper(CultureInfo.CurrentCulture);
            source = source.Where(x =>
            {
                return (x.DisplayName != null && x.DisplayName.ToUpper(CultureInfo.CurrentCulture).Contains(input.FilterText, StringComparison.CurrentCulture)) || x.Name.ToUpper(CultureInfo.CurrentCulture).Contains(input.FilterText, StringComparison.CurrentCulture)
|| (x.DefaultCultureName != null && x.DefaultCultureName.Contains(input.FilterText, StringComparison.CurrentCulture));
            }).ToList();
        }

        return new PagedResultDto<TemplateDefinitionDto>(source.Count, source.Skip(input.SkipCount).Take(input.MaxResultCount).ToList());
    }

    public virtual async Task<TemplateDefinitionDto> GetAsync(string name)
    {
        var templateDefinition = await TemplateDefinitionManager.GetAsync(name);
        var result = ObjectMapper.Map<TemplateDefinition, TemplateDefinitionDto>(templateDefinition);
        result.DisplayName = templateDefinition.GetLocalizedDisplayName(StringLocalizerFactory);
        return result;
    }
}
