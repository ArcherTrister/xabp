// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp;
using Volo.Abp.MongoDB;
using Volo.CmsKit;
using Volo.CmsKit.MongoDB;

using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Pro.MongoDB;

public static class CmsKitProMongoDbContextExtensions
{
    public static void ConfigurePro(this IMongoModelBuilder builder)
    {
        Check.NotNull(builder, nameof(builder));

        builder.ConfigureCmsKit();

        builder.Entity<NewsletterRecord>(x => x.CollectionName = AbpCmsKitDbProperties.DbTablePrefix + "NewsletterRecords");
        builder.Entity<ShortenedUrl>(x => x.CollectionName = AbpCmsKitDbProperties.DbTablePrefix + "ShortenedUrls");
        builder.Entity<Poll>(x => x.CollectionName = AbpCmsKitDbProperties.DbTablePrefix + "Polls");
        builder.Entity<PageFeedback>(x => x.CollectionName = AbpCmsKitDbProperties.DbTablePrefix + "PageFeedbacks");
        builder.Entity<PageFeedbackSetting>(x => x.CollectionName = AbpCmsKitDbProperties.DbTablePrefix + "PageFeedbackSettings");
    }
}
