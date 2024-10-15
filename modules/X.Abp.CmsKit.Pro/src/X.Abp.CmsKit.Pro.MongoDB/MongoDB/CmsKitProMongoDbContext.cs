using MongoDB.Driver;

using Volo.Abp.Data;
using Volo.Abp.MongoDB;
using Volo.CmsKit;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;

using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Pro.MongoDB;

[ConnectionStringName(AbpCmsKitDbProperties.ConnectionStringName)]
public class CmsKitProMongoDbContext : AbpMongoDbContext, ICmsKitProMongoDbContext
{
    /* Add mongo collections here. Example:
     * public IMongoCollection<Question> Questions => Collection<Question>();
     */
    public IMongoCollection<NewsletterRecord> NewsletterRecords => Collection<NewsletterRecord>();

    public IMongoCollection<ShortenedUrl> ShortenedUrls => Collection<ShortenedUrl>();

    public IMongoCollection<Poll> Polls => Collection<Poll>();

    public IMongoCollection<PollUserVote> PollUserVotes => Collection<PollUserVote>();

    public IMongoCollection<PageFeedback> PageFeedbacks => Collection<PageFeedback>();

    public IMongoCollection<PageFeedbackSetting> PageFeedbackSettings => Collection<PageFeedbackSetting>();

    protected override void CreateModel(IMongoModelBuilder modelBuilder)
    {
        base.CreateModel(modelBuilder);

        modelBuilder.ConfigurePro();
    }
}
