// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using X.Abp.LanguageManagement.Dto;

namespace X.Abp.LanguageManagement.Web;

public class AbpLanguageManagementWebAutoMapperProfile : Profile
{
    public AbpLanguageManagementWebAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<LanguageDto, Pages.LanguageManagement.EditModel.LanguageEditModalView>().MapExtraProperties();
        CreateMap<Pages.LanguageManagement.EditModel.LanguageEditModalView, UpdateLanguageDto>().MapExtraProperties();
        CreateMap<Pages.LanguageManagement.CreateModel.LanguageCreateModalView, CreateLanguageDto>().MapExtraProperties();
    }
}
