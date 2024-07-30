// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

using X.Abp.CmsKit.Polls;

namespace X.Abp.CmsKit.Admin.Polls;

[Serializable]
public class CreatePollDto
{
    [Required]
    [DynamicMaxLength(typeof(PollConst), "MaxQuestionLength")]
    public string Question { get; set; }

    [Required]
    [DynamicMaxLength(typeof(PollConst), "MaxCodeLength")]
    public string Code { get; set; }

    [DynamicMaxLength(typeof(PollConst), "MaxWidgetNameLength")]
    public string Widget { get; set; }

    [DynamicMaxLength(typeof(PollConst), "MaxNameLength")]
    public string Name { get; set; }

    public bool AllowMultipleVote { get; set; }

    public bool ShowVoteCount { get; set; }

    public bool ShowResultWithoutGivingVote { get; set; }

    public bool ShowHoursLeft { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? ResultShowingEndDate { get; set; }

    public Collection<PollOptionDto> PollOptions { get; set; } = new Collection<PollOptionDto>();
}
