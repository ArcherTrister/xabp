// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using X.Abp.Forms.Choices;

namespace X.Abp.Forms.Questions.ChoosableItems;

public interface IChoosable
{
    public void AddChoice(Guid id, int index, string value, bool isCorrect = false);

    public void AddChoices(List<(Guid Id, string Value, bool IsCorrect)> choices);

    public void MoveChoice(Guid id, int newIndex);

    public ICollection<Choice> GetChoices();

    public void ClearChoices();

    // public void SetChoiceValues(List<string> values);
}
