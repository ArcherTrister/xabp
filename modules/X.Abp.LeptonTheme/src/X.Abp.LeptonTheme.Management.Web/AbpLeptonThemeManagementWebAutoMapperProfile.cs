// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using X.Abp.LeptonTheme.Management.Web.Areas.LeptonThemeManagement.Models;

namespace X.Abp.LeptonTheme.Management.Web
{
    public class AbpLeptonThemeManagementWebAutoMapperProfile : Profile
    {
        public AbpLeptonThemeManagementWebAutoMapperProfile()
        {
            CreateMap<LeptonThemeSettingsDto, LeptonThemeSettingsViewModel>();
        }
    }
}
