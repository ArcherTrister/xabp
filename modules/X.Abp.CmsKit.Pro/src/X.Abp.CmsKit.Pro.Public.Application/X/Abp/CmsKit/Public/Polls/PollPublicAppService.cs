// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;
using Volo.Abp.Users;

using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;
using X.Abp.CmsKit.Polls;

namespace X.Abp.CmsKit.Public.Polls;

[RequiresFeature(CmsKitProFeatures.PollEnable)]
[RequiresGlobalFeature(typeof(PollsFeature))]
public class PollPublicAppService : PublicAppService, IPollPublicAppService
{
    protected CmsKitPollingOptions CmsKitPollingOptions { get; }

    protected IPollRepository PollRepository { get; }

    protected IPollUserVoteRepository PollUserVoteRepository { get; }

    protected PollManager PollManager { get; }

    public PollPublicAppService(
      IOptions<CmsKitPollingOptions> cmsKitPollingOptions,
      IPollRepository pollRepository,
      IPollUserVoteRepository pollUserVoteRepository,
      PollManager pollManager)
    {
        CmsKitPollingOptions = cmsKitPollingOptions.Value;
        PollRepository = pollRepository;
        PollUserVoteRepository = pollUserVoteRepository;
        PollManager = pollManager;
    }

    public Task<bool> IsWidgetNameAvailableAsync(string widgetName)
    {
        return Task.FromResult(CmsKitPollingOptions.WidgetNames.Contains(widgetName));
    }

/*    public virtual async Task<PollWithDetailsDto> FindByWidgetAsync(string widgetName)
    {
        var poll = await PollRepository.FindByWidgetAsync(widgetName);
        if (poll == null)
        {
            return null;
        }

        poll.OrderPollOptions();
        return ObjectMapper.Map<Poll, PollWithDetailsDto>(poll);
    }*/

    public virtual async Task<PollWithDetailsDto> FindByAvailableWidgetAsync(string widgetName)
    {
        var poll = await PollRepository.FindByAvailableWidgetAsync(widgetName, Clock.Now);
        if (poll == null)
        {
            return null;
        }

        poll.OrderPollOptions();
        return ObjectMapper.Map<Poll, PollWithDetailsDto>(poll);
    }

    public virtual async Task<PollWithDetailsDto> FindByCodeAsync(string code)
    {
        var poll = await PollRepository.FindByCodeAsync(code);
        if (poll == null)
        {
            return null;
        }

        poll.OrderPollOptions();
        return ObjectMapper.Map<Poll, PollWithDetailsDto>(poll);
    }

    public virtual async Task<GetResultDto> GetResultAsync(Guid id)
    {
        var keyValuePair = (await PollRepository.GetPollWithPollUserVotesAsync(id)).First();
        var pollResultDtoList = new List<PollResultDto>();
        var key = keyValuePair.Key;
        var source = keyValuePair.Value;

        var guidList = CurrentUser.IsAuthenticated ? source.Where(p => p.UserId == CurrentUser.GetId()).Select(v => v.PollOptionId).ToList() : new List<Guid>();
        key.OrderPollOptions();
        foreach (var pollOption in key.PollOptions)
        {
            pollResultDtoList.Add(new PollResultDto()
            {
                Text = pollOption.Text,
                VoteCount = pollOption.VoteCount,
                IsSelectedForCurrentUser = guidList.Contains(pollOption.Id)
            });
        }

        return new GetResultDto()
        {
            PollVoteCount = key.VoteCount,
            Question = key.Question,
            PollResultDetails = pollResultDtoList
        };
    }

    [Authorize]
    public virtual async Task SubmitVoteAsync(Guid id, SubmitPollInput input)
    {
        await PollManager.SubmitVoteAsync(id, CurrentUser.GetId(), input.PollOptionIds);
    }
}
