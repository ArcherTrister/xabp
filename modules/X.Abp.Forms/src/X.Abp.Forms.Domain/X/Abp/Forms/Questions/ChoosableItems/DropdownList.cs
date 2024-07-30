// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using X.Abp.Forms.Choices;

namespace X.Abp.Forms.Questions.ChoosableItems;

[QuestionType(QuestionTypes.DropdownList)]
public class DropdownList : QuestionBase, IChoosable, IRequired
{
    public virtual bool IsRequired { get; set; } = false;

    public virtual Collection<Choice> Choices { get; private set; }

    protected DropdownList()
    {
    }

    public DropdownList(Guid id, Guid? tenantId = null)
        : base(id, tenantId)
    {
        Choices = new Collection<Choice>();
    }

    public ICollection<Choice> GetChoices()
    {
        return Choices.OrderBy(q => q.Index).ToList();
    }

    public void ClearChoices()
    {
        Choices.Clear();
    }

    public void AddChoice(Guid id, int index, string value, bool isCorrect = false)
    {
        Choices.Add(new Choice(id, Id, index, value, isCorrect));
    }

    public void AddChoice(Guid id, string value, bool isCorrect = false)
    {
        AddChoice(id, Choices.Count - 1, value, isCorrect);
    }

    public void AddChoices(List<(Guid Id, string Value, bool IsCorrect)> choices)
    {
        for (var i = 0; i < choices.Count; i++)
        {
            AddChoice(choices[i].Id, i + 1, choices[i].Value, choices[i].IsCorrect);
        }
    }

    public void MoveChoice(Guid choiceId, int newIndex)
    {
        var choice = Choices.First(q => q.Id == choiceId);
        choice.UpdateIndex(newIndex);

        for (var i = newIndex; i < Choices.Count; i++)
        {
            Choices[i].UpdateIndex(i++);
        }
    }
}
