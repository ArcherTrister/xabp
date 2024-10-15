// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.GlobalFeatures;
using Volo.CmsKit;
using Volo.CmsKit.EntityFrameworkCore;

using X.Abp.CmsKit.GlobalFeatures;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.EntityFrameworkCore;

public static class CmsKitProDbContextModelBuilderExtensions
{
    public static void ConfigureCmsKitPro(this ModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.ConfigureCmsKit();

        if (GlobalFeatureManager.Instance.IsEnabled<NewslettersFeature>())
        {
            builder.Entity<NewsletterRecord>(b =>
            {
                b.ToTable(AbpCmsKitDbProperties.DbTablePrefix + "NewsletterRecords", AbpCmsKitDbProperties.DbSchema);
                b.ConfigureByConvention();
                b.Property(newsletterRecord => newsletterRecord.EmailAddress).HasMaxLength(NewsletterRecordConst.MaxEmailAddressLength).IsRequired(true).HasColumnName(nameof(NewsletterRecord.EmailAddress));
                b.HasIndex(newsletterRecord => new
                {
                    newsletterRecord.TenantId,
                    newsletterRecord.EmailAddress
                });
                b.HasMany(newsletterRecord => newsletterRecord.Preferences).WithOne().HasForeignKey(newsletterPreference => newsletterPreference.NewsletterRecordId).IsRequired(true);
                b.ApplyObjectExtensionMappings();
            });
            builder.Entity<NewsletterPreference>(b =>
            {
                b.ToTable(AbpCmsKitDbProperties.DbTablePrefix + "NewsletterPreferences", AbpCmsKitDbProperties.DbSchema);
                b.ConfigureByConvention();
                b.Property(newsletterPreference => newsletterPreference.Preference).HasMaxLength(NewsletterPreferenceConst.MaxPreferenceLength).IsRequired(true).HasColumnName(nameof(NewsletterPreference.Preference));
                b.Property(newsletterPreference => newsletterPreference.Source).HasMaxLength(NewsletterPreferenceConst.MaxSourceLength).IsRequired(true).HasColumnName(nameof(NewsletterPreference.Source));
                b.Property(newsletterPreference => newsletterPreference.SourceUrl).HasMaxLength(NewsletterPreferenceConst.MaxSourceUrlLength).IsRequired(true).HasColumnName(nameof(NewsletterPreference.SourceUrl));
                b.HasIndex(newsletterPreference => new
                {
                    newsletterPreference.TenantId,
                    newsletterPreference.Preference,
                    newsletterPreference.Source
                });
                b.ApplyObjectExtensionMappings();
            });
        }

        if (GlobalFeatureManager.Instance.IsEnabled<UrlShortingFeature>())
        {
            builder.Entity<ShortenedUrl>(b =>
            {
                b.ToTable(AbpCmsKitDbProperties.DbTablePrefix + "ShortenedUrls", AbpCmsKitDbProperties.DbSchema);
                b.ConfigureByConvention();
                b.Property(shortenedUrl => shortenedUrl.Source).HasMaxLength(ShortenedUrlConst.MaxSourceLength).IsRequired(true).HasColumnName(nameof(ShortenedUrl.Source));
                b.Property(shortenedUrl => shortenedUrl.Target).HasMaxLength(ShortenedUrlConst.MaxTargetLength).IsRequired(true).HasColumnName(nameof(ShortenedUrl.Target));
                b.HasIndex(shortenedUrl => new
                {
                    shortenedUrl.TenantId,
                    shortenedUrl.Source
                });
                b.ApplyObjectExtensionMappings();
            });
        }

        if (GlobalFeatureManager.Instance.IsEnabled<PollsFeature>())
        {
            builder.Entity<Poll>(b =>
            {
                b.ToTable(AbpCmsKitDbProperties.DbTablePrefix + "Polls", AbpCmsKitDbProperties.DbSchema);
                b.Property(poll => poll.Question).HasMaxLength(PollConst.MaxQuestionLength).IsRequired(true).HasColumnName(nameof(Poll.Question));
                b.Property(poll => poll.Code).HasMaxLength(PollConst.MaxCodeLength).IsRequired(true).HasColumnName(nameof(Poll.Code));
                b.Property(poll => poll.Widget).HasMaxLength(PollConst.MaxWidgetNameLength).HasColumnName(nameof(Poll.Widget));
                b.Property(poll => poll.Name).HasMaxLength(PollConst.MaxNameLength).HasColumnName(nameof(Poll.Name));
                b.ConfigureByConvention();
                b.ApplyObjectExtensionMappings();
            });
            builder.Entity<PollUserVote>(b =>
            {
                b.ToTable(AbpCmsKitDbProperties.DbTablePrefix + "PollUserVotes", AbpCmsKitDbProperties.DbSchema);
                b.ConfigureByConvention();
                b.ApplyObjectExtensionMappings();
            });
            builder.Entity<PollOption>(b =>
            {
                b.ToTable(AbpCmsKitDbProperties.DbTablePrefix + "PollOptions", AbpCmsKitDbProperties.DbSchema);
                b.Property(pollOption => pollOption.Text).HasMaxLength(PollConst.MaxTextLength).IsRequired(true).HasColumnName(nameof(PollOption.Text));
                b.ConfigureByConvention();
                b.ApplyObjectExtensionMappings();
            });
        }

        if (GlobalFeatureManager.Instance.IsEnabled<PageFeedbackFeature>())
        {
            builder.Entity<PageFeedback>(b =>
            {
                b.ToTable(AbpCmsKitDbProperties.DbTablePrefix + "PageFeedbacks", AbpCmsKitDbProperties.DbSchema);
                b.Property(pageFeedback => pageFeedback.Url).HasMaxLength(PageFeedbackConst.MaxUrlLength).HasColumnName(nameof(PageFeedback.Url));
                b.Property(pageFeedback => pageFeedback.EntityType).HasMaxLength(PageFeedbackConst.MaxEntityTypeLength).IsRequired(true).HasColumnName(nameof(PageFeedback.EntityType));
                b.Property(pageFeedback => pageFeedback.EntityId).HasMaxLength(PageFeedbackConst.MaxEntityIdLength).HasColumnName(nameof(PageFeedback.EntityId));
                b.Property(pageFeedback => pageFeedback.UserNote).HasMaxLength(PageFeedbackConst.MaxUserNoteLength).HasColumnName(nameof(PageFeedback.UserNote));
                b.Property(pageFeedback => pageFeedback.AdminNote).HasMaxLength(PageFeedbackConst.MaxUrlLength).HasColumnName(nameof(PageFeedback.AdminNote));
                b.Property(pageFeedback => pageFeedback.IsUseful).HasColumnName(nameof(PageFeedback.IsUseful));
                b.Property(pageFeedback => pageFeedback.IsHandled).HasColumnName(nameof(PageFeedback.IsHandled));
                b.ConfigureByConvention();
                b.ApplyObjectExtensionMappings();
            });
            builder.Entity<PageFeedbackSetting>(b =>
            {
                b.ToTable(AbpCmsKitDbProperties.DbTablePrefix + "PageFeedbackSettings", AbpCmsKitDbProperties.DbSchema);
                b.Property(pageFeedbackSetting => pageFeedbackSetting.EntityType).HasMaxLength(PageFeedbackConst.MaxEntityTypeLength).HasColumnName(nameof(PageFeedbackSetting.EntityType));
                b.Property(pageFeedbackSetting => pageFeedbackSetting.EmailAddresses).HasMaxLength(PageFeedbackConst.MaxEmailAddressesLength).HasColumnName(nameof(PageFeedbackSetting.EmailAddresses));
                b.HasIndex(pageFeedbackSetting => new
                {
                    pageFeedbackSetting.TenantId,
                    pageFeedbackSetting.EntityType
                }).IsUnique(true);
                b.ConfigureByConvention();
                b.ApplyObjectExtensionMappings();
            });
        }

        builder.TryConfigureObjectExtensions<CmsKitProDbContext>();
    }
}
