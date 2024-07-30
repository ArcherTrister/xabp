// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

using X.Abp.CmsKit.Public.Polls;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Poll;

[Widget(AutoInitialize = true, RefreshUrl = "/CmsKitProPublicWidgets/Poll", ScriptFiles = new string[] { "/Pages/Public/Shared/Components/Poll/Poll.js" }, ScriptTypes = new Type[] { typeof(PollWidgetScriptContributor) })]
[ViewComponent(Name = "CmsPoll")]
public class PollViewComponent : AbpViewComponent
{
    protected IPollPublicAppService PollPublicAppService { get; }

    protected IOptions<Volo.Abp.AspNetCore.Mvc.UI.AbpMvcUiOptions> AbpMvcUiOptions { get; }

    public PollViewComponent(
      IPollPublicAppService pollPublicAppService,
      IOptions<Volo.Abp.AspNetCore.Mvc.UI.AbpMvcUiOptions> options)
    {
        PollPublicAppService = pollPublicAppService;
        AbpMvcUiOptions = options;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync(string widgetName)
    {
        var pollWithDetails = await PollPublicAppService.FindByWidgetAsync(widgetName);
        return pollWithDetails == null
            ? View("~/Pages/Public/Shared/Components/Poll/Empty.cshtml", new PollViewModel()
            {
                Code = widgetName
            })
            : CheckShowingEndDate(pollWithDetails)
            ? await new PollByCodeViewComponent(PollPublicAppService, AbpMvcUiOptions).InvokeAsync(pollWithDetails.Code, widgetName, HttpContext.Request.Path.ToString())
            : View("~/Pages/Public/Shared/Components/Poll/Default.cshtml", new PollViewModel()
            {
                Id = null
            });
    }

    private static bool CheckShowingEndDate(PollWithDetailsDto pollWithDetails)
    {
        var now = DateTime.Now;
        if (pollWithDetails.StartDate > now)
        {
            return false;
        }

        var resultShowingEndDate = pollWithDetails.ResultShowingEndDate;
        if (resultShowingEndDate.HasValue)
        {
            resultShowingEndDate = pollWithDetails.ResultShowingEndDate;
            if (resultShowingEndDate.Value < now)
            {
                return false;
            }
        }

        return true;
    }
}
