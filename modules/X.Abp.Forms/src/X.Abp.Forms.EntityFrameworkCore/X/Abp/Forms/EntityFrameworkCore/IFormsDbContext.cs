// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;

using X.Abp.Forms.Answers;
using X.Abp.Forms.Choices;
using X.Abp.Forms.Forms;
using X.Abp.Forms.Questions;
using X.Abp.Forms.Questions.ChoosableItems;
using X.Abp.Forms.Responses;

namespace X.Abp.Forms.EntityFrameworkCore;

[ConnectionStringName(FormsDbProperties.ConnectionStringName)]
public interface IFormsDbContext : IEfCoreDbContext
{
    /* Add DbSet for each Aggregate Root here. Example:
     * DbSet<Question> Questions { get; }
     */
    public DbSet<Form> Forms { get; }

    public DbSet<QuestionBase> Questions { get; }

    public DbSet<ShortText> ShortTexts { get; }

    public DbSet<Choice> Choices { get; }

    public DbSet<Checkbox> Checkboxes { get; }

    public DbSet<ChoiceMultiple> ChoiceMultiples { get; }

    public DbSet<DropdownList> DropdownLists { get; }

    public DbSet<Answer> Answers { get; }

    public DbSet<FormResponse> FormResponses { get; }
}
