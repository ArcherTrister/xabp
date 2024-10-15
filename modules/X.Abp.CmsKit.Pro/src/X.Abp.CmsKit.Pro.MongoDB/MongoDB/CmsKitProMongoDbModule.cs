using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.CmsKit.MongoDB;

using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;

using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Pro.MongoDB;

[DependsOn(
    typeof(CmsKitProDomainModule),
    typeof(CmsKitMongoDbModule)
    )]
public class CmsKitProMongoDbModule : AbpModule
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
        });
    }
}
