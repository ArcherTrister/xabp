// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.CmsKit.MongoDB;

using X.Abp.CmsKit.Faqs;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;

using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Pro.MongoDB;

[DependsOn(
    typeof(AbpCmsKitProDomainModule),
    typeof(CmsKitMongoDbModule))]
public class AbpCmsKitProMongoDbModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMongoDbContext<CmsKitProMongoDbContext>(options =>
        {
            /* Add custom repositories here. Example:
             * options.AddRepository<Question, MongoQuestionRepository>();
             */
            options.AddRepository<NewsletterRecord, MongoNewsletterRecordRepository>();
            options.AddRepository<ShortenedUrl, MongoShortenedUrlRepository>();
            options.AddRepository<Poll, MongoPollRepository>();
            options.AddRepository<PollUserVote, MongoPollUserVoteRepository>();
            options.AddRepository<PageFeedbackSetting, MongoPageFeedbackSettingRepository>();
            options.AddRepository<PageFeedback, MongoPageFeedbackRepository>();
            options.AddRepository<FaqSection, MongoFaqSectionRepository>();
            options.AddRepository<FaqQuestion, MongoFaqQuestionRepository>();
        });
    }
}
