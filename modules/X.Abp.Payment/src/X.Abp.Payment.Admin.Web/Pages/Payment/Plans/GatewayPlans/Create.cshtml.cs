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
using X.Abp.Payment.Web;

namespace X.Abp.Payment.Admin.Web.Pages.Payment.Plans.GatewayPlans
{
    public class CreateModalModel : PaymentPageModel
    {
        public List<SelectListItem> SelectableGateways { get; protected set; }

        [BindProperty(SupportsGet = true)]
        [HiddenInput]
        public Guid PlanId { get; set; }

        [BindProperty]
        public GatewayPlanCreateViewModel GatewayPlan { get; set; }

        protected IPlanAdminAppService PlanAdminAppService { get; }

        protected IGatewayAppService GatewayAppService { get; }

        protected IOptions<PaymentWebOptions> PaymentWebOptions { get; }

        public CreateModalModel(
      IPlanAdminAppService planAdminAppService,
      IGatewayAppService gatewayAppService,
      IOptions<PaymentWebOptions> paymentWebOptions)
        {
            PlanAdminAppService = planAdminAppService;
            GatewayAppService = gatewayAppService;
            PaymentWebOptions = paymentWebOptions;
        }

        public virtual async Task OnGetAsync()
        {
            GatewayPlan = new GatewayPlanCreateViewModel();
            SelectableGateways = (await GatewayAppService.GetSubscriptionSupportedGatewaysAsync()).Select(g => new SelectListItem(g.Name, g.DisplayName)).ToList();
        }

        public virtual async Task<IActionResult> OnPostAsync()
        {
            GatewayPlanCreateInput input = ObjectMapper.Map<GatewayPlanCreateViewModel, GatewayPlanCreateInput>(GatewayPlan);
            await PlanAdminAppService.CreateGatewayPlanAsync(PlanId, input);
            return NoContent();
        }
    }
}
