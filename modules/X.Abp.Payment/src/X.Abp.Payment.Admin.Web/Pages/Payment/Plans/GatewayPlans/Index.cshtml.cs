// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Payment.Admin.Plans;
using X.Abp.Payment.Plans;

namespace X.Abp.Payment.Admin.Web.Pages.Payment.Plans.GatewayPlans
{
    public class IndexModel : PaymentPageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public PlanDto Plan { get; set; }

        protected IPlanAdminAppService PlanAdminAppService { get; }

        public IndexModel(IPlanAdminAppService planAdminAppService)
        {
            PlanAdminAppService = planAdminAppService;
        }

        public async Task OnGetAsync()
        {
            Plan = await PlanAdminAppService.GetAsync(Id);
        }
    }
}
