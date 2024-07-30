// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using X.Abp.Forms.Choices;

namespace X.Abp.Forms.Questions.ChoosableItems;

[QuestionType(QuestionTypes.Checkbox)]
public class Checkbox : QuestionBase, IChoosable, IRequired, IHasOtherOption
{
    public virtual bool IsRequired { get; set; } = false;

    public virtual bool HasOtherOption { get; set; } = false;

    public virtual Collection<Choice> Choices { get; private set; }

    protected Checkbox()
    {
    }

    public Checkbox(Guid id, Guid? tenantId = null)
        : base(id, tenantId)
    {
        Choices = new Collection<Choice>();
    }

    public void AddChoice(Guid id, string value, bool isCorrect = false)
    {
        AddChoice(id: id, index: Choices.Count - 1, value: value, isCorrect: isCorrect);
    }

    public void AddChoice(Guid id, int index, string value, bool isCorrect = false)
    {
        Choices.Add(new Choice(id, Id, index, value, isCorrect));
    }

    public void AddChoices(List<(Guid Id, string Value, bool IsCorrect)> choices)
    {
        for (var i = 0; i < choices.Count; i++)
        {
            AddChoice(id: choices[i].Id, index: i + 1, value: choices[i].Value, isCorrect: choices[i].IsCorrect);
        }
    }

    public void AddChoiceOther(Guid id, int index = 0)
    {
        SetOtherOption(true);
        var newIndex = index == 0 ? Choices.Count + 1 : index;
        AddChoice(id: id, index: newIndex, ChoiceConsts.OtherChoice);
    }

    public ICollection<Choice> GetChoices()
    {
        return Choices.OrderBy(q => q.Index).ToList();
    }

    public void ClearChoices()
    {
        Choices.Clear();
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
