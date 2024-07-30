// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.CmsKit.Admin.Polls;

[Serializable]
public class PollWithDetailsDto : EntityDto<Guid>, IHasCreationTime, IMultiTenant
{
    public string Question { get; set; }

    public string Code { get; set; }

    public string Widget { get; set; }

    public string Name { get; set; }

    public DateTime StartDate { get; set; }

    public bool AllowMultipleVote { get; set; }

    public int VoteCount { get; set; }

    public bool ShowVoteCount { get; set; }

    public bool ShowResultWithoutGivingVote { get; set; }

    public bool ShowHoursLeft { get; set; }

    public DateTime? EndDate { get; set; }

    public DateTime? ResultShowingEndDate { get; set; }

    public Guid? TenantId { get; set; }

    public DateTime CreationTime { get; set; }

    public List<PollOptionDto> PollOptions { get; set; } = new List<PollOptionDto>();
}
