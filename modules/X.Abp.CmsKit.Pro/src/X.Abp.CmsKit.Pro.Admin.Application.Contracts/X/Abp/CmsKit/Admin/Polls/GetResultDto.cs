// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

namespace X.Abp.CmsKit.Admin.Polls;

[Serializable]
public class GetResultDto : EntityDto
{
    public string Question { get; set; }

    public int PollVoteCount { get; set; }

    public List<PollResultDto> PollResultDetails { get; set; }
}
