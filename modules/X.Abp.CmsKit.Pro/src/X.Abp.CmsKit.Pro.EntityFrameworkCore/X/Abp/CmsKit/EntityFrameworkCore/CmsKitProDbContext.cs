// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.CmsKit;

using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.EntityFrameworkCore;

[ConnectionStringName(AbpCmsKitDbProperties.ConnectionStringName)]
public class CmsKitProDbContext : AbpDbContext<CmsKitProDbContext>, ICmsKitProDbContext
{
    public DbSet<NewsletterRecord> NewsletterRecords { get; set; }

    public DbSet<NewsletterPreference> NewsletterPreferences { get; set; }

    public DbSet<ShortenedUrl> ShortenedUrls { get; set; }

    public DbSet<Poll> Polls { get; set; }

    public DbSet<PollUserVote> PollUserVotes { get; set; }

    public DbSet<PollOption> PollOptions { get; set; }

    public DbSet<PageFeedback> PageFeedbacks { get; set; }

    public DbSet<PageFeedbackSetting> PageFeedbackSettings { get; set; }

    public CmsKitProDbContext(DbContextOptions<CmsKitProDbContext> options)
      : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ConfigureCmsKitPro();
    }
}
