// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Payment.Admin.Plans;
using X.Abp.Payment.Plans;

namespace X.Abp.Payment.Admin.Web.Pages.Payment.Plans
{
  public class UpdateModalModel : PaymentPageModel
  {
    protected IPlanAdminAppService PlanAdminAppService { get; }

    [HiddenInput]
    [BindProperty(SupportsGet = true)]
    public Guid Id { get; set; }

    [BindProperty]
    public PlanUpdateViewModel Plan { get; set; }

    public UpdateModalModel(IPlanAdminAppService planAdminAppService)
    {
      PlanAdminAppService = planAdminAppService;
    }

    public virtual async Task OnGetAsync()
    {
      PlanDto planDto = await PlanAdminAppService.GetAsync(Id);
      Plan = ObjectMapper.Map<PlanDto, PlanUpdateViewModel>(planDto);
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
      ValidateModel();
      PlanUpdateInput planUpdateInput = ObjectMapper.Map<PlanUpdateViewModel, PlanUpdateInput>(Plan);
      await PlanAdminAppService.UpdateAsync(Id, planUpdateInput);
      return NoContent();
    }
  }
}
