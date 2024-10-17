// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AutoMapper;

using Volo.Abp.AutoMapper;
using Volo.CmsKit.Admin.Tags;
using Volo.CmsKit.Tags;

using X.Abp.CmsKit.Admin.Faqs;
using X.Abp.CmsKit.Admin.Newsletters;
using X.Abp.CmsKit.Admin.PageFeedbacks;
using X.Abp.CmsKit.Admin.Polls;
using X.Abp.CmsKit.Admin.UrlShorting;
using X.Abp.CmsKit.Faqs;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.PageFeedbacks;
using X.Abp.CmsKit.Polls;
using X.Abp.CmsKit.UrlShorting;

namespace X.Abp.CmsKit.Admin;

public class AbpCmsKitProAdminApplicationAutoMapperProfile : Profile
{
    public AbpCmsKitProAdminApplicationAutoMapperProfile()
    {
        CreateMap<NewsletterRecord, NewsletterRecordWithDetailsDto>()
            .ForMember(recordWithDetailsDto => recordWithDetailsDto.Preferences, c => c.MapFrom(newsletterRecord => newsletterRecord.Preferences))
            .MapExtraProperties();
        CreateMap<NewsletterPreference, NewsletterPreferenceDto>();
        CreateMap<NewsletterSummaryQueryResultItem, NewsletterRecordDto>().Ignore(newsletterRecordDto => newsletterRecordDto.ExtraProperties);
        CreateMap<TagDto, TagCreateDto>().MapExtraProperties();
        CreateMap<TagDto, TagUpdateDto>().MapExtraProperties();
        CreateMap<TagCreateDto, Tag>(MemberList.Source).Ignore(tag => tag.Id).MapExtraProperties();
        CreateMap<TagUpdateDto, Tag>(MemberList.Source).MapExtraProperties();
        CreateMap<ShortenedUrl, ShortenedUrlDto>();

        CreateMap<Poll, PollDto>().MapExtraProperties();
        CreateMap<Poll, PollWithDetailsDto>().MapExtraProperties();
        CreateMap<PollOption, PollOptionDto>();
        CreateMap<PageFeedback, PageFeedbackDto>();
        CreateMap<PageFeedbackDto, UpdatePageFeedbackDto>();
        CreateMap<PageFeedbackSetting, PageFeedbackSettingDto>();
        CreateMap<FaqSection, FaqSectionDto>();
        CreateMap<FaqSectionWithQuestionCount, FaqSectionWithQuestionCountDto>();
        CreateMap<FaqQuestion, FaqQuestionDto>();
        CreateMap<FaqGroupInfo, FaqGroupInfoDto>();
    }
}
