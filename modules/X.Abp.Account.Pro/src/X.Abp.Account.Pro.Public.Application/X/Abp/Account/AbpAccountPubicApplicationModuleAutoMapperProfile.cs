// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using AutoMapper;

using Microsoft.AspNetCore.Identity;

using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;

using X.Abp.Account.Dtos;
using X.Abp.Account.ExternalProviders;
using X.Abp.Identity;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account;

public class AbpAccountPubicApplicationModuleAutoMapperProfile : Profile
{
    public AbpAccountPubicApplicationModuleAutoMapperProfile()
    {
        CreateMap<ExternalProviderSettings, ExternalProviderItemDto>(MemberList.Destination);

        CreateMap<ExternalProviderSettings, ExternalProviderItemWithSecretDto>(MemberList.Destination)
        .ForMember(d => d.Success, opt => opt.MapFrom(x => !x.Name.IsNullOrWhiteSpace()));

        CreateMap<IdentityUser, ProfileDto>()
        .ForMember(dest => dest.HasPassword,
            op => op.MapFrom(src => src.PasswordHash != null))
        .MapExtraProperties();

        CreateMap<IdentityUser, IdentityUserDto>()
        .MapExtraProperties()
        .ForMember(dest => dest.LockoutEnd, src => src.MapFrom<DateTime?>(r => r.LockoutEnd.HasValue ? r.LockoutEnd.Value.DateTime : null))
        .Ignore(x => x.IsLockedOut)
        .Ignore(x => x.SupportTwoFactor)
        .Ignore(x => x.RoleNames);

        CreateMap<IdentityUser, UserLookupDto>()
        .ForMember(dest => dest.UserName, src => src.MapFrom(identityUser => string.Format("{0} ({1})", identityUser.UserName, identityUser.Email)));

        CreateMap<IdentitySecurityLog, IdentitySecurityLogDto>();

        CreateMap<UserLoginInfo, UserLoginInfoDto>();

        CreateMap<ExternalProviderSettings, AuthenticationSchemeDto>();
    }
}
