// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using X.Abp.Forms.Answers;
using X.Abp.Forms.Choices;

namespace X.Abp.Forms.Questions;

public class QuestionWithAnswers
{
    public QuestionBase Question { get; set; }

    public List<Answer> Answers { get; set; }

    public List<Choice> Choices { get; set; }

    public QuestionWithAnswers()
    {
    }
}
