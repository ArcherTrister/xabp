// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;
using Volo.Abp.Localization;

namespace X.Abp.LanguageManagement;

public class AbpLanguageManagementDomainAutoMapperProfile : Profile
{
    public AbpLanguageManagementDomainAutoMapperProfile()
    {
        CreateMap<Language, LanguageInfo>()
            .Ignore(p => p.TwoLetterISOLanguageName);
        CreateMap<Language, LanguageEto>();
        CreateMap<LanguageText, LanguageTextEto>();
    }
}
