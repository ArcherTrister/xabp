// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using X.Abp.VersionManagement.AppEditions;
using X.Abp.VersionManagement.AppEditions.Dtos;

namespace X.Abp.VersionManagement;

public class AbpVersionManagementApplicationAutoMapperProfile : Profile
{
    public AbpVersionManagementApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<AppEdition, AppEditionDto>();
    }
}
