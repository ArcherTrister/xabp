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
public class FormsDbContext : AbpDbContext<FormsDbContext>, IFormsDbContext
{
    public virtual DbSet<Form> Forms { get; set; }

    public virtual DbSet<QuestionBase> Questions { get; set; }

    public virtual DbSet<ShortText> ShortTexts { get; set; }

    public virtual DbSet<Choice> Choices { get; set; }

    public virtual DbSet<Checkbox> Checkboxes { get; set; }

    public virtual DbSet<ChoiceMultiple> ChoiceMultiples { get; set; }

    public virtual DbSet<DropdownList> DropdownLists { get; set; }

    public virtual DbSet<Answer> Answers { get; set; }

    public virtual DbSet<FormResponse> FormResponses { get; set; }

    public FormsDbContext(DbContextOptions<FormsDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ConfigureForms();
    }
}
