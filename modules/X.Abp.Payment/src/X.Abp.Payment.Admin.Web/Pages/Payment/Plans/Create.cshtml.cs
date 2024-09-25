// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using X.Abp.Payment.Admin.Plans;

namespace X.Abp.Payment.Admin.Web.Pages.Payment.Plans
{
  public class CreateModalModel : PaymentPageModel
  {
    protected IPlanAdminAppService PlanAdminAppService { get; }

    [BindProperty]
    public PlanCreateViewModel Plan { get; set; }

    public CreateModalModel(IPlanAdminAppService planAdminAppService)
    {
      PlanAdminAppService = planAdminAppService;
    }

    public virtual Task<IActionResult> OnGetAsync()
    {
      Plan = new PlanCreateViewModel();
      return Task.FromResult<IActionResult>(Page());
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
      ValidateModel();
      PlanCreateInput planCreateInput = ObjectMapper.Map<PlanCreateViewModel, PlanCreateInput>(Plan);
      await PlanAdminAppService.CreateAsync(planCreateInput);
      return NoContent();
    }
  }
}
