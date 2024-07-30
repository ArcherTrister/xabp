// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;
using Volo.Abp.TextTemplating;

using X.Abp.TextTemplateManagement.TextTemplates;

namespace X.Abp.TextTemplateManagement;

public class AbpTextTemplateManagementApplicationAutoMapperProfile : Profile
{
    public AbpTextTemplateManagementApplicationAutoMapperProfile()
    {
        CreateMap<TemplateDefinition, TemplateDefinitionDto>().Ignore(x => x.DisplayName);
        CreateMap<TextTemplateContent, TextTemplateContentDto>();
    }
}
