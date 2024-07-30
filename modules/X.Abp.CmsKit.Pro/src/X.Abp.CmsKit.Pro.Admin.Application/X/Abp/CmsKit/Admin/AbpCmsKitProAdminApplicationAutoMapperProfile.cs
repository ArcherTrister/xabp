// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;
using Volo.CmsKit.Admin.Tags;
using Volo.CmsKit.Tags;

using X.Abp.CmsKit.Admin.Newsletters;
using X.Abp.CmsKit.Admin.Polls;
using X.Abp.CmsKit.Admin.UrlShorting;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.Polls;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Admin;

public class AbpCmsKitProAdminApplicationAutoMapperProfile : Profile
{
    public AbpCmsKitProAdminApplicationAutoMapperProfile()
    {
        CreateMap<NewsletterRecord, NewsletterRecordWithDetailsDto>().ForMember(recordWithDetailsDto => recordWithDetailsDto.Preferences, c => c.MapFrom(newsletterRecord => newsletterRecord.Preferences));
        CreateMap<NewsletterPreference, NewsletterPreferenceDto>();
        CreateMap<NewsletterSummaryQueryResultItem, NewsletterRecordDto>();
        CreateMap<NewsletterSummaryQueryResultItem, NewsletterRecordCsvDto>().Ignore(newsletterRecordCsvDto => newsletterRecordCsvDto.SecurityCode);
        CreateMap<TagDto, TagCreateDto>();
        CreateMap<TagDto, TagUpdateDto>();
        CreateMap<TagCreateDto, Tag>(MemberList.Source).Ignore(p => p.Id);
        CreateMap<TagUpdateDto, Tag>(MemberList.Source);
        CreateMap<ShortenedUrl, ShortenedUrlDto>();
        CreateMap<Poll, PollDto>();
        CreateMap<Poll, PollWithDetailsDto>();
        CreateMap<PollOption, PollOptionDto>();
    }
}
