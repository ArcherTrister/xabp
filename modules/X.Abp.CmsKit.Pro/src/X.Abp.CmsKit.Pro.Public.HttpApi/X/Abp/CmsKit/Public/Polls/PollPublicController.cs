// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Public.Polls;

[RemoteService(true, Name = AbpCmsKitProPublicRemoteServiceConsts.RemoteServiceName)]
[Area(AbpCmsKitProPublicRemoteServiceConsts.ModuleName)]
[Route("api/cms-kit-public/poll")]
[RequiresGlobalFeature(typeof(PollsFeature))]
[RequiresFeature(CmsKitProFeatures.PollEnable)]
public class PollPublicController : CmsKitProPublicController, IPollPublicAppService
{
    protected IPollPublicAppService PollPublicAppService { get; }

    public PollPublicController(IPollPublicAppService pollPublicAppService)
    {
        PollPublicAppService = pollPublicAppService;
    }

    [Route("widget-name-available")]
    [HttpGet]
    public virtual async Task<bool> IsWidgetNameAvailableAsync(string widgetName)
    {
        return await PollPublicAppService.IsWidgetNameAvailableAsync(widgetName);
    }

    [Route("by-available-widget-name")]
    [HttpGet]
    public virtual async Task<PollWithDetailsDto> FindByAvailableWidgetAsync(string widgetName)
    {
        return await PollPublicAppService.FindByAvailableWidgetAsync(widgetName);
    }

    [Route("by-code")]
    [HttpGet]
    public virtual async Task<PollWithDetailsDto> FindByCodeAsync(string code)
    {
        return await PollPublicAppService.FindByCodeAsync(code);
    }

    [HttpGet]
    [Route("result/{id}")]
    public virtual async Task<GetResultDto> GetResultAsync(Guid id)
    {
        return await PollPublicAppService.GetResultAsync(id);
    }

    [Route("{id}")]
    [HttpPost]
    public virtual async Task SubmitVoteAsync(Guid id, SubmitPollInput input)
    {
        await PollPublicAppService.SubmitVoteAsync(id, input);
    }
}
