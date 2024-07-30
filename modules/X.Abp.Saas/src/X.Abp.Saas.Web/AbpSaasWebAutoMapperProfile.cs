// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;

using X.Abp.Saas.Dtos;

namespace X.Abp.Saas.Web;

public class AbpSaasWebAutoMapperProfile : Profile
{
    public AbpSaasWebAutoMapperProfile()
    {
        CreateMap<SaasTenantDto, Pages.Saas.Tenants.EditModalModel.TenantInfoModel>()
            .MapExtraProperties();

        CreateMap<Pages.Saas.Tenants.CreateModalModel.TenantConnectionStringsModel, SaasTenantConnectionStringsDto>()
            .MapExtraProperties();

        CreateMap<Pages.Saas.Tenants.CreateModalModel.TenantDatabaseConnectionStringsModel, SaasTenantDatabaseConnectionStringsDto>()
            .MapExtraProperties();

        CreateMap<Pages.Saas.Tenants.CreateModalModel.TenantInfoModel, SaasTenantCreateDto>()
            .Ignore(t => t.EditionEndDateUtc)
            .MapExtraProperties();

        CreateMap<Pages.Saas.Tenants.EditModalModel.TenantInfoModel, SaasTenantUpdateDto>()
            .Ignore(t => t.EditionEndDateUtc)
            .MapExtraProperties();

        CreateMap<Pages.Saas.Tenants.ConnectionStringsModalModel.TenantConnectionStringsModel, SaasTenantConnectionStringsDto>()
            .MapExtraProperties()
            .ReverseMap()
            .Ignore(c => c.UseSharedDatabase)
            .Ignore(c => c.UseSpecificDatabase);

        CreateMap<Pages.Saas.Tenants.ConnectionStringsModalModel.TenantDatabaseConnectionStringsModel, SaasTenantDatabaseConnectionStringsDto>()
            .MapExtraProperties().ReverseMap();

        CreateMap<EditionDto, Pages.Saas.Editions.EditModalModel.EditionInfoModel>()
            .MapExtraProperties();

        CreateMap<Pages.Saas.Editions.CreateModalModel.EditionInfoModel, EditionCreateDto>()
            .MapExtraProperties();

        CreateMap<Pages.Saas.Editions.EditModalModel.EditionInfoModel, EditionUpdateDto>()
            .MapExtraProperties();
    }
}
