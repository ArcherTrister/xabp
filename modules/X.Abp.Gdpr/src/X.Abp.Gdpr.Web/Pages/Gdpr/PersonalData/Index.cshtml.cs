// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace X.Abp.Gdpr.Web.Pages.Gdpr.PersonalData;

[Authorize]
public class IndexModel : GdprPageModel
{
  public bool IsNewRequestAllowed { get; set; }

  protected IGdprRequestAppService GdprRequestAppService { get; }

  public IndexModel(IGdprRequestAppService gdprRequestAppService)
  {
    GdprRequestAppService = gdprRequestAppService;
  }

  public virtual async Task OnGetAsync()
  {
    IsNewRequestAllowed = await GdprRequestAppService.IsNewRequestAllowedAsync();
  }
}
