// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.ObjectModel;

using Volo.Abp.Application.Dtos;

using X.Abp.Forms.Answers;

namespace X.Abp.Forms.Responses;

public class UpdateResponseDto : EntityDto<Guid>
{
    public Guid FormId { get; set; }

    public string Email { get; set; }

    public Collection<AnswerDto> Answers { get; set; }
}
