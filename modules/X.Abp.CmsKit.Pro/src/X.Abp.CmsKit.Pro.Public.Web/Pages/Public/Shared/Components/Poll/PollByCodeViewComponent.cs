// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

using X.Abp.CmsKit.Public.Polls;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Poll;

[Widget(AutoInitialize = true, RefreshUrl = "/CmsKitProPublicWidgets/PollByCode", ScriptFiles = new string[] { "/Pages/Public/Shared/Components/Poll/PollByCode.js" }, ScriptTypes = new Type[] { typeof(PollWidgetScriptContributor) })]
[ViewComponent(Name = "CmsPollByCode")]
public class PollByCodeViewComponent : AbpViewComponent
{
    protected IPollPublicAppService PollPublicAppService { get; }

    protected IOptions<Volo.Abp.AspNetCore.Mvc.UI.AbpMvcUiOptions> AbpMvcUiOptions { get; }

    public PollByCodeViewComponent(
      IPollPublicAppService pollPublicAppService,
      IOptions<Volo.Abp.AspNetCore.Mvc.UI.AbpMvcUiOptions> options)
    {
        PollPublicAppService = pollPublicAppService;
        AbpMvcUiOptions = options;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync(
      string code,
      string widgetName = null,
      string returnUrl = null)
    {
        var poll = await PollPublicAppService.FindByCodeAsync(code);
        if (poll == null)
        {
            return View("~/Pages/Public/Shared/Components/Poll/Empty.cshtml", new PollViewModel()
            {
                Code = code
            });
        }

        if (returnUrl.IsNullOrEmpty())
        {
            returnUrl = HttpContext.Request.Path.ToString();
        }

        var getResultDto = await PollPublicAppService.GetResultAsync(poll.Id);

        var model = new PollViewModel()
        {
            Id = poll.Id,
            Code = code,
            Name = poll.Name,
            WidgetName = widgetName ?? code,
            Question = poll.Question,
            ShowResultWithoutGivingVote = poll.ShowResultWithoutGivingVote,
            AllowMultipleVote = poll.AllowMultipleVote,
            ShowHoursLeft = poll.ShowHoursLeft,
            ShowVoteCount = poll.ShowVoteCount,
            VoteCount = poll.VoteCount,
            Texts = poll.PollOptions.Select(p => p.Text).ToList(),
            OptionIds = poll.PollOptions.Select(p => p.Id).ToList(),
            IsVoted = getResultDto.PollResultDetails.Any(p => p.IsSelectedForCurrentUser),
            EndDate = poll.EndDate,
            ResultShowingEndDate = poll.ResultShowingEndDate,
            PollVoteCount = getResultDto.PollVoteCount,
            PollResultDetails = getResultDto.PollResultDetails,
            LoginUrl = $"{AbpMvcUiOptions.Value.LoginUrl}?returnUrl={returnUrl}"
        };
        return View("~/Pages/Public/Shared/Components/Poll/Default.cshtml", model);
    }
}
