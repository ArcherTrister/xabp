// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Forms.Questions;

[QuestionType(QuestionTypes.ShortText)]
public class ShortText : QuestionBase, IRequired
{
    public bool IsRequired { get; set; }

    protected ShortText()
    {
    }

    public ShortText(Guid id, Guid? tenantId = null)
        : base(id, tenantId)
    {
    }
}
