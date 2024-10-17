// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;
using X.Abp.CmsKit.Polls;

namespace X.Abp.CmsKit.Admin.Polls;

[Authorize(AbpCmsKitProAdminPermissions.Polls.Default)]
[RequiresFeature(CmsKitProFeatures.PollEnable)]
[RequiresGlobalFeature(typeof(PollsFeature))]
public class PollAdminAppService : CmsKitProAdminAppServiceBase, IPollAdminAppService
{
    protected CmsKitPollingOptions CmsKitPollingOptions { get; }

    protected IPollRepository PollRepository { get; }

    protected PollManager PollManager { get; }

    public PollAdminAppService(
      IPollRepository pollRepository,
      IOptions<CmsKitPollingOptions> pollingOptions,
      PollManager pollManager)
    {
        CmsKitPollingOptions = pollingOptions.Value;
        PollRepository = pollRepository;
        PollManager = pollManager;
    }

    public virtual async Task<PollWithDetailsDto> GetAsync(Guid id)
    {
        var poll = await PollRepository.GetAsync(id, true);
        poll.OrderPollOptions();
        return ObjectMapper.Map<Poll, PollWithDetailsDto>(poll);
    }

    public virtual async Task<PagedResultDto<PollDto>> GetListAsync(GetPollListInput input)
    {
        var pollList = await PollRepository.GetListAsync(input.Filter, input.Sorting, input.SkipCount, input.MaxResultCount);
        return new PagedResultDto<PollDto>(await PollRepository.GetCountAsync(input.Filter), ObjectMapper.Map<List<Poll>, List<PollDto>>(pollList));
    }

    [Authorize(AbpCmsKitProAdminPermissions.Polls.Create)]
    public virtual async Task<PollWithDetailsDto> CreateAsync(CreatePollDto input)
    {
        if (!input.Widget.IsNullOrWhiteSpace())
        {
            await SetWidgetToNullAsync(input.Widget);
        }

        await PollManager.EnsureExistAsync(input.Code);
        var poll1 = new Poll(GuidGenerator.Create(), input.Question, input.Code, input.Widget, input.Name, input.StartDate, input.AllowMultipleVote, input.ShowVoteCount, input.ShowResultWithoutGivingVote, input.ShowHoursLeft, input.EndDate, input.ResultShowingEndDate, CurrentTenant?.Id);
        foreach (var pollOption in input.PollOptions)
        {
            poll1.AddPollOption(GuidGenerator.Create(), pollOption.Text, pollOption.Order, CurrentTenant?.Id);
        }

        var poll2 = await PollRepository.InsertAsync(poll1, false);
        return ObjectMapper.Map<Poll, PollWithDetailsDto>(poll2);
    }

    [Authorize(AbpCmsKitProAdminPermissions.Polls.Update)]
    public virtual async Task<PollWithDetailsDto> UpdateAsync(Guid id, UpdatePollDto input)
    {
        var poll = await PollRepository.GetAsync(id, true);

        if (poll.Code != input.Code)
        {
            await PollManager.EnsureExistAsync(input.Code);
        }

        if (!input.Widget.IsNullOrWhiteSpace() && poll.Widget != input.Widget)
        {
            await SetWidgetToNullAsync(input.Widget);
        }

        poll.SetQuestion(input.Question);
        poll.SetCode(input.Code);
        poll.ShowVoteCount = input.ShowVoteCount;
        poll.ShowResultWithoutGivingVote = input.ShowResultWithoutGivingVote;
        poll.ShowHoursLeft = input.ShowHoursLeft;
        poll.Widget = input.Widget;
        poll.Name = input.Name;
        poll.SetDates(input.StartDate, input.EndDate, input.ResultShowingEndDate);
        foreach (var optionId in poll.PollOptions.Select(p => p.Id).Except(input.PollOptions.Select(p => p.Id)).ToList())
        {
            poll.RemovePollOption(optionId);
        }

        foreach (var pollOption in input.PollOptions)
        {
            poll.UpdatePollOption(pollOption.Id, pollOption.Text, pollOption.Order, poll.TenantId);
        }

        await PollRepository.UpdateAsync(poll, false);
        return ObjectMapper.Map<Poll, PollWithDetailsDto>(poll);
    }

    [Authorize(AbpCmsKitProAdminPermissions.Polls.Delete)]
    public virtual Task DeleteAsync(Guid id)
    {
        return PollRepository.DeleteAsync(id, false);
    }

    public virtual Task<ListResultDto<PollWidgetDto>> GetWidgetsAsync()
    {
        return Task.FromResult(new ListResultDto<PollWidgetDto>()
        {
            Items = CmsKitPollingOptions.WidgetNames.Select(n => new PollWidgetDto()
            {
                Name = n
            }).ToList()
        });
    }

    public virtual async Task<GetResultDto> GetResultAsync(Guid id)
    {
        var poll = await PollRepository.GetAsync(id, true);
        poll.OrderPollOptions();
        var pollResultDtoList = new List<PollResultDto>();
        foreach (var pollOption1 in poll.PollOptions)
        {
            var pollOption = pollOption1;
            pollResultDtoList.Add(new PollResultDto()
            {
                Text = pollOption.Text,
                VoteCount = poll.PollOptions.First(p => p.Id == pollOption.Id).VoteCount
            });
        }

        return new GetResultDto()
        {
            PollVoteCount = poll.VoteCount,
            Question = poll.Question,
            PollResultDetails = pollResultDtoList
        };
    }

    private async Task SetWidgetToNullAsync(string widget)
    {
        foreach (var item in await PollRepository.GetListByWidgetAsync(widget))
        {
            item.Widget = null;
            await PollRepository.UpdateAsync(item, false);
        }
    }
}
