// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using AutoMapper;

using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;

namespace X.Abp.Identity;

public class AbpIdentityProApplicationModuleAutoMapperProfile : Profile
{
    public AbpIdentityProApplicationModuleAutoMapperProfile()
    {
        CreateMap<IdentityUser, IdentityUserDto>()
            .MapExtraProperties()
            .ForMember(dest => dest.LockoutEnd, src => src.MapFrom<DateTime?>(r => r.LockoutEnd.HasValue ? r.LockoutEnd.Value.DateTime : null))
            .Ignore(x => x.IsLockedOut)
            .Ignore(x => x.SupportTwoFactor)
            .Ignore(x => x.RoleNames);

        CreateMap<IdentityRole, IdentityRoleDto>()
            .MapExtraProperties();

        CreateMap<IdentityClaimType, ClaimTypeDto>()
            .MapExtraProperties()
            .Ignore(x => x.ValueTypeAsString);

        CreateMap<IdentityUserClaim, IdentityUserClaimDto>();

        CreateMap<IdentityUserClaimDto, IdentityUserClaim>()
            .Ignore(x => x.TenantId)
            .Ignore(x => x.Id);

        CreateMap<IdentityRoleClaim, IdentityRoleClaimDto>();

        CreateMap<IdentityRoleClaimDto, IdentityRoleClaim>()
            .Ignore(x => x.TenantId)
            .Ignore(x => x.Id);

        CreateMap<CreateClaimTypeDto, IdentityClaimType>()
            .MapExtraProperties()
            .Ignore(x => x.IsStatic)
            .Ignore(x => x.Id);

        CreateMap<UpdateClaimTypeDto, IdentityClaimType>()
            .MapExtraProperties()
            .Ignore(x => x.IsStatic)
            .Ignore(x => x.Id);

        CreateMap<OrganizationUnit, OrganizationUnitDto>()
            .MapExtraProperties();

        CreateMap<OrganizationUnitRole, OrganizationUnitRoleDto>();

        CreateMap<OrganizationUnit, OrganizationUnitWithDetailsDto>()
            .MapExtraProperties()
            .Ignore(x => x.Roles);

        CreateMap<IdentityRole, OrganizationUnitRoleDto>()
            .ForMember(dest => dest.RoleId, src => src.MapFrom(r => r.Id));

        CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();

        CreateMap<IdentityRole, IdentityRoleLookupDto>();

        CreateMap<OrganizationUnit, OrganizationUnitLookupDto>();
    }
}
