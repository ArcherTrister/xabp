// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Dtos;

namespace X.Abp.Forms.Choices;

public class ChoiceDto : EntityDto<Guid>
{
    public int Index { get; set; }

    public bool IsCorrect { get; set; }

    public string Value { get; set; }
}
