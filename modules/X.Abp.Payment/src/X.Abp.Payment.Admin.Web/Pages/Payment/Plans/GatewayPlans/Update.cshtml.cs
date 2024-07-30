// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

using X.Abp.Payment.Admin.Plans;
using X.Abp.Payment.Gateways;
using X.Abp.Payment.Plans;
using X.Abp.Payment.Web;

namespace X.Abp.Payment.Admin.Web.Pages.Payment.Plans.GatewayPlans
{
    public class UpdateModalModel : PaymentPageModel
    {
        public List<SelectListItem> SelectableGateways { get; protected set; }

        [BindProperty(SupportsGet = true)]
        [HiddenInput]
        public Guid PlanId { get; set; }

        [HiddenInput]
        [BindProperty(SupportsGet = true)]
        public string Gateway { get; set; }

        [BindProperty]
        public GatewayPlansUpdateViewModel GatewayPlan { get; set; }

        protected IPlanAdminAppService PlanAdminAppService { get; }

        protected IPlanAppService PlanAppService { get; }

        protected IGatewayAppService GatewayAppService { get; }

        protected IOptions<PaymentWebOptions> PaymentWebOptions { get; }

        public UpdateModalModel(
          IPlanAdminAppService planAdminAppService,
          IPlanAppService planAppService,
          IGatewayAppService gatewayAppService,
          IOptions<PaymentWebOptions> paymentWebOptions)
        {
            PlanAdminAppService = planAdminAppService;
            PlanAppService = planAppService;
            GatewayAppService = gatewayAppService;
            PaymentWebOptions = paymentWebOptions;
        }

        public virtual async Task OnGetAsync()
        {
            List<GatewayDto> source = await GatewayAppService.GetSubscriptionSupportedGatewaysAsync();
            SelectableGateways = source.Select(g => new SelectListItem(g.Name, g.DisplayName)).ToList();
            GatewayPlanDto gatewayPlanDto = await PlanAppService.GetGatewayPlanAsync(PlanId, Gateway);
            GatewayPlan = ObjectMapper.Map<GatewayPlanDto, GatewayPlansUpdateViewModel>(gatewayPlanDto);
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            GatewayPlanUpdateInput input = ObjectMapper.Map<GatewayPlansUpdateViewModel, GatewayPlanUpdateInput>(GatewayPlan);
            await PlanAdminAppService.UpdateGatewayPlanAsync(PlanId, Gateway, input);
            return NoContent();
        }
    }
}
