// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

namespace X.Abp.Forms.Answers;

public class AnswerDto : EntityDto<Guid>
{
    public Guid QuestionId { get; set; }

    public Guid? ChoiceId { get; set; }

    public Guid FormResponseId { get; set; }

    public DateTime AnswerDate { get; set; }

    public string Value { get; set; }
}
