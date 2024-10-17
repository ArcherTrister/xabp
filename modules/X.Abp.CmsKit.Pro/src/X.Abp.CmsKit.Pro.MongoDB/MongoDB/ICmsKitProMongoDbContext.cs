// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using MongoDB.Driver;

using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.CmsKit;

using X.Abp.CmsKit.Faqs;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;

using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Pro.MongoDB;

[ConnectionStringName(AbpCmsKitDbProperties.ConnectionStringName)]
public interface ICmsKitProMongoDbContext : IAbpMongoDbContext
{
    /* Define mongo collections here. Example:
     * IMongoCollection<Question> Questions { get; }
     */
    IMongoCollection<NewsletterRecord> NewsletterRecords { get; }

    IMongoCollection<ShortenedUrl> ShortenedUrls { get; }

    IMongoCollection<Poll> Polls { get; }

    IMongoCollection<PollUserVote> PollUserVotes { get; }

    IMongoCollection<PageFeedbackSetting> PageFeedbackSettings { get; }

    IMongoCollection<PageFeedback> PageFeedbacks { get; }

    IMongoCollection<FaqSection> FaqSections { get; }

    IMongoCollection<FaqQuestion> FaqQuestions { get; }
}
