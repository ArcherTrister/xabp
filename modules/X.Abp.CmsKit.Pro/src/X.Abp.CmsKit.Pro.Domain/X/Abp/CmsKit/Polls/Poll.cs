// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.ObjectModel;
using System.Linq;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.Polls;

public class Poll : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual string Question { get; protected set; }

    public virtual string Code { get; protected set; }

    public virtual string Widget { get; set; }

    public virtual string Name { get; set; }

    public virtual bool AllowMultipleVote { get; protected set; }

    public virtual int VoteCount { get; protected set; }

    public virtual bool ShowVoteCount { get; set; }

    public virtual bool ShowResultWithoutGivingVote { get; set; }

    public virtual bool ShowHoursLeft { get; set; }

    public virtual DateTime StartDate { get; protected set; }

    public virtual DateTime? EndDate { get; protected set; }

    public virtual DateTime? ResultShowingEndDate { get; protected set; }

    public virtual Guid? TenantId { get; protected set; }

    public virtual Collection<PollOption> PollOptions { get; protected set; }

    protected Poll()
    {
    }

    public Poll(
      Guid id,
      string question,
      string code,
      string widget,
      string name,
      DateTime startDate,
      bool allowMultipleVote = false,
      bool showVoteCount = true,
      bool showResultWithoutGivingVote = true,
      bool showHoursLeft = true,
      DateTime? endDate = null,
      DateTime? resultShowingEndDate = null,
      Guid? tenantId = null)
      : base(id)
    {
        SetQuestion(question);
        SetCode(code);
        SetDates(startDate, endDate, resultShowingEndDate);
        Widget = widget;
        Name = name;
        AllowMultipleVote = allowMultipleVote;
        ShowVoteCount = showVoteCount;
        ShowResultWithoutGivingVote = showResultWithoutGivingVote;
        ShowHoursLeft = showHoursLeft;
        TenantId = tenantId;
        PollOptions = new Collection<PollOption>();
    }

    public virtual PollOption AddPollOption(
      Guid optionId,
      string text,
      int order,
      Guid? tenantId)
    {
        var pollOption = new PollOption(optionId, text, order, Id, tenantId);
        PollOptions.Add(pollOption);
        return pollOption;
    }

    public virtual void UpdatePollOption(Guid optionId, string text, int order, Guid? tenantId)
    {
        var pollOption = PollOptions.SingleOrDefault(p => p.Id == optionId);
        if (pollOption != null)
        {
            pollOption.SetText(text);
            pollOption.SetOrder(order);
        }
        else
        {
            PollOptions.Add(new PollOption(optionId, text, order, Id, tenantId));
        }
    }

    public virtual void RemovePollOption(Guid optionId)
    {
        var pollOption = PollOptions.SingleOrDefault(p => p.Id == optionId);
        PollOptions.Remove(pollOption);
        Decrease(pollOption.VoteCount);
    }

    public virtual void SetQuestion(string question)
    {
        Question = Check.NotNullOrEmpty(question, nameof(question), PollConst.MaxQuestionLength, 0);
    }

    public virtual void SetCode(string code)
    {
        Code = Check.NotNullOrEmpty(code, nameof(code), PollConst.MaxCodeLength, 0);
    }

    public virtual void Increase()
    {
        ++VoteCount;
    }

    public virtual void Decrease(int voteCount)
    {
        VoteCount -= voteCount;
    }

    public virtual void SetDates(DateTime startDate, DateTime? endDate = null, DateTime? resultShowingEndDate = null)
    {
        StartDate = startDate;
        if (endDate != null && endDate <= startDate)
        {
            throw new PollEndDateCannotSetBeforeStartDateException(startDate, endDate.GetValueOrDefault());
        }

        EndDate = endDate;
        if (resultShowingEndDate != null && resultShowingEndDate <= startDate)
        {
            throw new PollResultShowingEndDateCannotSetBeforeStartDateException(startDate, resultShowingEndDate.GetValueOrDefault());
        }

        ResultShowingEndDate = resultShowingEndDate;
    }

    public virtual void OrderPollOptions()
    {
        PollOptions = new Collection<PollOption>(PollOptions.OrderBy(p => p.Order).ToList());
    }
}
