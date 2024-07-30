// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using X.Abp.Forms.Answers;
using X.Abp.Forms.Questions;

namespace X.Abp.Forms.Forms;

public class FormWithAnswers
{
    public Form Form { get; set; }

    public List<QuestionBase> Items { get; set; }

    public List<Answer> Answers { get; set; }
}
