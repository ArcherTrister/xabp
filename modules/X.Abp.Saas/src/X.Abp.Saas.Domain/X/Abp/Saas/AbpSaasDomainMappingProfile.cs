// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

using X.Abp.Saas.Editions;
using X.Abp.Saas.Tenants;

namespace X.Abp.Saas;

public class AbpSaasDomainMappingProfile : Profile
{
    public AbpSaasDomainMappingProfile()
    {
        CreateMap<Tenant, TenantConfiguration>()
            .ForMember(ti => ti.ConnectionStrings, opts =>
            {
                opts.MapFrom((tenant, ti) =>
                {
                    var connectionStrings = new ConnectionStrings();

                    foreach (var tenantConnectionString in tenant.ConnectionStrings)
                    {
                        connectionStrings[tenantConnectionString.Name] = tenantConnectionString.Value;
                    }

                    return connectionStrings;
                });
            }).ForMember(x => x.IsActive, y => y.Ignore());

        CreateMap<Edition, EditionEto>();
        CreateMap<Tenant, TenantEto>();
    }
}
