// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;

using X.Abp.Account.Dtos;
using X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;

namespace X.Abp.Account.Public.Web;

public class AbpAccountPublicWebAutomapperProfile : Profile
{
    public AbpAccountPublicWebAutomapperProfile()
    {
        CreateMap<ProfileDto, AccountProfilePersonalInfoManagementGroupViewComponent.PersonalInfoModel>().Ignore(p => p.TimeZoneItems).MapExtraProperties();
    }
}
