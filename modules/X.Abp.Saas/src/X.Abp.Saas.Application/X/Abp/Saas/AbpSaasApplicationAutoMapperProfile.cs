// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;

using X.Abp.Saas.Dtos;
using X.Abp.Saas.Editions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas;

public class AbpSaasApplicationAutoMapperProfile : Profile
{
    public AbpSaasApplicationAutoMapperProfile()
    {
        CreateMap<Tenant, SaasTenantDto>().MapExtraProperties().Ignore(x => x.EditionName)
                        .ForMember(dest => dest.HasDefaultConnectionString,
                        op => op.MapFrom(src => src.FindDefaultConnectionString != null));

        CreateMap<Edition, EditionDto>().MapExtraProperties().Ignore(x => x.PlanName);

        CreateMap<Edition, EditionLookupDto>().MapExtraProperties();
    }
}
