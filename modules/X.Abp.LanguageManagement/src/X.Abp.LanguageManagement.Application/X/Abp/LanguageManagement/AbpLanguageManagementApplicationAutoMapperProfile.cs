// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;

using X.Abp.LanguageManagement.Dto;

namespace X.Abp.LanguageManagement;

public class AbpLanguageManagementApplicationAutoMapperProfile : Profile
{
    public AbpLanguageManagementApplicationAutoMapperProfile()
    {
        CreateMap<Language, LanguageDto>()
            .MapExtraProperties()
            .Ignore(x => x.IsDefaultLanguage);
    }
}
