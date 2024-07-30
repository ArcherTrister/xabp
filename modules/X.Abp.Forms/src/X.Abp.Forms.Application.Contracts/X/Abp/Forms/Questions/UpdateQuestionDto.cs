// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using X.Abp.Forms.Choices;

namespace X.Abp.Forms.Questions;

public class UpdateQuestionDto
{
    [Required]
    public int Index { get; set; }

    [Required]
    public string Title { get; set; }

    public string Description { get; set; }

    public bool IsRequired { get; set; }

    public bool HasOtherOption { get; set; }

    [Required]
    public QuestionTypes QuestionType { get; set; }

    public List<ChoiceDto> Choices { get; set; } = new();
}
