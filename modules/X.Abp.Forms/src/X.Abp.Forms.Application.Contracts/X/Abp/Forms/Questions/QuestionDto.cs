// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.Application.Dtos;

using X.Abp.Forms.Choices;

namespace X.Abp.Forms.Questions;

public class QuestionDto : FullAuditedEntityDto<Guid>
{
    public Guid FormId { get; set; }

    public int Index { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public bool IsRequired { get; set; }

    public bool HasOtherOption { get; set; }

    public QuestionTypes QuestionType { get; set; }

    public List<ChoiceDto> Choices { get; set; }
}
