﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Modularity;
using Volo.CmsKit.EntityFrameworkCore;

using X.Abp.CmsKit.Faqs;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.EntityFrameworkCore;

[DependsOn(
    typeof(AbpCmsKitProDomainModule),
    typeof(CmsKitEntityFrameworkCoreModule))]
public class AbpCmsKitProEntityFrameworkCoreModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAbpDbContext<CmsKitProDbContext>(options =>
        {
            options.AddDefaultRepositories<ICmsKitProDbContext>();

            options.AddRepository<NewsletterRecord, EfCoreNewsletterRecordRepository>();
            options.AddRepository<ShortenedUrl, EfCoreShortenedUrlRepository>();
            options.AddRepository<Poll, EfCorePollRepository>();
            options.AddRepository<PollUserVote, EfCorePollUserVoteRepository>();
            options.AddRepository<PageFeedback, EfCorePageFeedbackRepository>();
            options.AddRepository<PageFeedbackSetting, EfCorePageFeedbackSettingRepository>();
            options.AddRepository<FaqSection, EfCoreFaqSectionRepository>();
            options.AddRepository<FaqQuestion, EfCoreFaqQuestionRepository>();
        });
    }
}
