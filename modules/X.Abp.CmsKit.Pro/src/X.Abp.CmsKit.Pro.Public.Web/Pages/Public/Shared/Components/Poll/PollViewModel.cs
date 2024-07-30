// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc.Localization;

using X.Abp.CmsKit.Public.Polls;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Poll;

public class PollViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Question")]
    public string Question { get; set; }

    public bool AllowMultipleVote { get; set; }

    public int VoteCount { get; set; }

    public bool ShowVoteCount { get; set; }

    public bool ShowResultWithoutGivingVote { get; set; }

    public bool ShowHoursLeft { get; set; }

    public TimeSpan? TimeLeft => GetTimeLeft();

    public bool IsOutdated => CheckOutdated();

    public DateTime? EndDate { get; set; }

    public DateTime? ResultShowingEndDate { get; set; }

    public DateTime CreationTime { get; set; }

    public List<string> Texts { get; set; } = new List<string>();

    public List<Guid> OptionIds { get; set; } = new List<Guid>();

    public int PollVoteCount { get; set; }

    public List<PollResultDto> PollResultDetails { get; set; }

    public bool IsVoted { get; set; }

    public string WidgetName { get; set; }

    public string LoginUrl { get; set; }

    public string Code { get; set; }

    public string Name { get; set; }

    public string GetTimeLeftAsText(IHtmlLocalizer l)
    {
        if (!TimeLeft.HasValue || !ShowHoursLeft || TimeLeft.Value.TotalDays > 365.0)
        {
            return string.Empty;
        }

        if (TimeLeft.Value.TotalDays > 30.0)
        {
            var num = (int)(TimeLeft.Value.TotalDays / 30.0);
            var name = num == 1 ? "MonthLeft" : "MonthsLeft";
            return (string)l.GetString(name, num);
        }

        if (TimeLeft.Value.TotalDays > 1.0)
        {
            var totalDays = (int)TimeLeft.Value.TotalDays;
            var name = totalDays == 1 ? "DayLeft" : "DaysLeft";
            return (string)l.GetString(name, totalDays);
        }

        if (TimeLeft.Value.TotalHours > 1.0)
        {
            var totalHours = (int)TimeLeft.Value.TotalHours;
            var name = totalHours == 1 ? "HourLeft" : "HoursLeft";
            return (string)l.GetString(name, totalHours);
        }

        if (TimeLeft.Value.TotalMinutes > 1.0)
        {
            var totalMinutes = (int)TimeLeft.Value.TotalMinutes;
            var name = totalMinutes == 1 ? "MinuteLeft" : "MinutesLeft";
            return (string)l.GetString(name, totalMinutes);
        }

        var totalSeconds = (int)TimeLeft.Value.TotalSeconds;
        var name1 = totalSeconds == 1 ? "SecondLeft" : "SecondsLeft";
        return (string)l.GetString(name1, totalSeconds);
    }

    public string GetVoteCountText(IHtmlLocalizer l)
    {
        return !ShowVoteCount
            ? string.Empty
            : VoteCount == 0
            ? l["NoVotes"].Value
            : VoteCount == 1 ? (string)l.GetString("SingleVoteCount") : (string)l.GetString("VoteCount{0}", VoteCount);
    }

    private bool CheckOutdated()
    {
        return TimeLeft.HasValue && TimeLeft.Value.TotalSeconds < 1.0;
    }

    public TimeSpan? GetTimeLeft()
    {
        var now = DateTime.Now;
        if (!EndDate.HasValue)
        {
            return null;
        }

        if (EndDate.Value <= now)
        {
            return null;
        }

        var endDate = EndDate;
        var dateTime = now;
        return !endDate.HasValue ? null : new TimeSpan?(endDate.GetValueOrDefault() - dateTime);
    }
}
