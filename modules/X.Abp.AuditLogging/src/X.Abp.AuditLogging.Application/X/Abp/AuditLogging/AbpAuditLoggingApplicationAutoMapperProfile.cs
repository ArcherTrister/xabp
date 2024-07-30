// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AuditLogging;

using X.Abp.AuditLogging.Dtos;

namespace X.Abp.AuditLogging;

public class AbpAuditLoggingApplicationAutoMapperProfile : Profile
{
    public AbpAuditLoggingApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        CreateMap<AuditLog, AuditLogDto>()
            .MapExtraProperties();

        CreateMap<EntityChange, EntityChangeDto>()
            .MapExtraProperties();

        CreateMap<EntityChangeWithUsername, EntityChangeWithUsernameDto>();

        CreateMap<EntityPropertyChange, EntityPropertyChangeDto>();

        CreateMap<AuditLogAction, AuditLogActionDto>()
            .MapExtraProperties();
    }
}
