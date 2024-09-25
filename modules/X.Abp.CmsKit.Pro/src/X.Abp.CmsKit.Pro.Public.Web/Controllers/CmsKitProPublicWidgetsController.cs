// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;

using X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Contact;
using X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Poll;

namespace X.Abp.CmsKit.Pro.Public.Web.Controllers;

public class CmsKitProPublicWidgetsController : AbpController
{
  public IActionResult Contact()
  {
    return ViewComponent(typeof(ContactViewComponent));
  }

  public virtual async Task<IActionResult> Poll(string widgetName)
  {
    return await Task.FromResult(ViewComponent(typeof(PollViewComponent), new
    {
      widgetName
    }));
  }

  public virtual async Task<IActionResult> PollByCode(string code)
  {
    return await Task.FromResult(ViewComponent(typeof(PollByCodeViewComponent), new
    {
      code
    }));
  }
}
