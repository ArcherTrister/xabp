// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using X.Abp.Payment.Admin.Plans;
using X.Abp.Payment.Admin.Web.Pages.Payment.Plans;
using X.Abp.Payment.Admin.Web.Pages.Payment.Plans.GatewayPlans;
using X.Abp.Payment.Plans;

namespace X.Abp.Payment.Admin.Web
{
    public class AbpPaymentAdminWebAutoMapperProfile : Profile
    {
        public AbpPaymentAdminWebAutoMapperProfile()
        {
            CreateMap<GatewayPlanCreateViewModel, GatewayPlanCreateInput>().MapExtraProperties();
            CreateMap<GatewayPlanDto, GatewayPlansUpdateViewModel>().MapExtraProperties();
            CreateMap<GatewayPlansUpdateViewModel, GatewayPlanUpdateInput>().MapExtraProperties();
            CreateMap<PlanCreateViewModel, PlanCreateInput>().MapExtraProperties();
            CreateMap<PlanDto, PlanUpdateViewModel>().MapExtraProperties();
            CreateMap<PlanUpdateViewModel, PlanUpdateInput>().MapExtraProperties();
        }
    }
}
