// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

using X.Abp.Forms.Answers;
using X.Abp.Forms.Choices;
using X.Abp.Forms.Forms;
using X.Abp.Forms.Questions;
using X.Abp.Forms.Questions.ChoosableItems;
using X.Abp.Forms.Responses;

namespace X.Abp.Forms.EntityFrameworkCore;

public static class FormsDbContextModelBuilderExtensions
{
    public static void ConfigureForms(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.Entity<Form>(f =>
        {
            f.ToTable(FormsDbProperties.DbTablePrefix + "Forms", FormsDbProperties.DbSchema);
            f.ConfigureByConvention();

            f.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(FormConsts.MaxTitleLength)
                .HasColumnName(nameof(Form.Title));

            f.Property(t => t.Description)
                .HasMaxLength(FormConsts.MaxDescriptionLength)
                .HasColumnName(nameof(Form.Description));

            f.HasIndex(x => new { x.Id, x.TenantId });

            f.ApplyObjectExtensionMappings();
        });

        builder.Entity<QuestionBase>(i =>
        {
            i.ToTable(FormsDbProperties.DbTablePrefix + "Questions", FormsDbProperties.DbSchema);
            i.ConfigureByConvention();

            i.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(QuestionsConsts.MaxTitleLength)
                .HasColumnName(nameof(QuestionBase.Title));

            i.Property(t => t.Description)
                .HasMaxLength(QuestionsConsts.MaxDescriptionLength)
                .HasColumnName(nameof(QuestionBase.Description));

            i.HasDiscriminator<QuestionTypes>("Type")
                .HasValue<ShortText>(QuestionTypes.ShortText)
                .HasValue<Checkbox>(QuestionTypes.Checkbox)
                .HasValue<ChoiceMultiple>(QuestionTypes.ChoiceMultiple)
                .HasValue<DropdownList>(QuestionTypes.DropdownList);

            i.HasIndex(x => new { x.Id, x.FormId, x.TenantId });

            i.ApplyObjectExtensionMappings();
        });

        builder.Entity<ChoiceMultiple>(c =>
        {
            c.ConfigureByConvention();
            c.HasMany(ch => ch.Choices)
                .WithOne()
                .HasForeignKey(q => q.ChoosableQuestionId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired();

            c.Property(q => q.FormId)
                .HasColumnName("FormId");
            c.Property(q => q.IsRequired)
                .HasColumnName("IsRequired");
            c.Property(q => q.HasOtherOption)
                .HasColumnName("HasOtherOption");

            c.ApplyObjectExtensionMappings();
        });

        builder.Entity<Checkbox>(c =>
        {
            c.ConfigureByConvention();
            c.HasMany(ch => ch.Choices)
                .WithOne()
                .HasForeignKey(q => q.ChoosableQuestionId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired();

            c.Property(q => q.FormId)
                .HasColumnName("FormId");
            c.Property(q => q.IsRequired)
                .HasColumnName("IsRequired");
            c.Property(q => q.HasOtherOption)
                .HasColumnName("HasOtherOption");

            c.ApplyObjectExtensionMappings();
        });

        builder.Entity<DropdownList>(dd =>
        {
            dd.ConfigureByConvention();
            dd.HasMany(ch => ch.Choices)
                .WithOne()
                .HasForeignKey(q => q.ChoosableQuestionId)
                .OnDelete(DeleteBehavior.ClientCascade)
                .IsRequired();

            dd.Property(q => q.FormId)
                .HasColumnName("FormId");
            dd.Property(q => q.IsRequired)
                .HasColumnName("IsRequired");

            dd.ApplyObjectExtensionMappings();
        });

        builder.Entity<ShortText>(st =>
        {
            st.ConfigureByConvention();
            st.Property(q => q.FormId)
                .HasColumnName("FormId");
            st.Property(q => q.IsRequired)
                .HasColumnName("IsRequired");

            st.ApplyObjectExtensionMappings();
        });

        builder.Entity<Choice>(c =>
        {
            c.ConfigureByConvention();
            c.ToTable(FormsDbProperties.DbTablePrefix + "Choices", FormsDbProperties.DbSchema);
            c.Property(q => q.Value)
                .IsRequired()
                .HasMaxLength(ChoiceConsts.MaxValueLength)
                .HasColumnName(nameof(Choice.Value));

            c.HasIndex(x => new { x.Id, ChoosableItemId = x.ChoosableQuestionId, x.TenantId });

            c.ApplyObjectExtensionMappings();
        });

        builder.Entity<FormResponse>(u =>
        {
            u.ToTable(FormsDbProperties.DbTablePrefix + "FormResponses", FormsDbProperties.DbSchema);
            u.ConfigureByConvention();

            u.HasMany(ch => ch.Answers)
                .WithOne()
                .HasForeignKey(q => q.FormResponseId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            u.Property(x => x.Email).HasMaxLength(FormConsts.ResponseConsts.MaxEmailLength);

            u.HasIndex(x => new { x.Id, x.FormId, x.UserId, x.TenantId });

            u.ApplyObjectExtensionMappings();
        });

        builder.Entity<Answer>(f =>
        {
            f.ToTable(FormsDbProperties.DbTablePrefix + "Answers", FormsDbProperties.DbSchema);
            f.ConfigureByConvention();

            f.HasIndex(x => new { x.Id, x.QuestionId, x.FormResponseId, x.TenantId });

            f.ApplyObjectExtensionMappings();
        });

        builder.TryConfigureObjectExtensions<FormsDbContext>();
    }
}
