// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.CmsKit;

using X.Abp.CmsKit.Faqs;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.EntityFrameworkCore;

[ConnectionStringName(AbpCmsKitDbProperties.ConnectionStringName)]
public interface ICmsKitProDbContext : IEfCoreDbContext
{
    DbSet<NewsletterRecord> NewsletterRecords { get; }

    DbSet<NewsletterPreference> NewsletterPreferences { get; }

    DbSet<ShortenedUrl> ShortenedUrls { get; }

    DbSet<Poll> Polls { get; }

    DbSet<PollUserVote> PollUserVotes { get; }

    DbSet<PollOption> PollOptions { get; }

    DbSet<PageFeedback> PageFeedbacks { get; }

    DbSet<PageFeedbackSetting> PageFeedbackSettings { get; }

    DbSet<FaqQuestion> FaqQuestions { get; set; }

    DbSet<FaqSection> FaqSections { get; set; }
}
