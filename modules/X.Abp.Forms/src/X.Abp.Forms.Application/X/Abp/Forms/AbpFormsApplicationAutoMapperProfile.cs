// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;

using AutoMapper;

using Volo.Abp.AutoMapper;

using X.Abp.Forms.Answers;
using X.Abp.Forms.Choices;
using X.Abp.Forms.Forms;
using X.Abp.Forms.Questions;
using X.Abp.Forms.Questions.ChoosableItems;
using X.Abp.Forms.Responses;

namespace X.Abp.Forms;

public class AbpFormsApplicationAutoMapperProfile : Profile
{
    public AbpFormsApplicationAutoMapperProfile()
    {
        CreateMap<FormResponse, FormResponseDto>()
            .Ignore(q => q.Answers);

        CreateMap<FormWithResponse, FormWithResponseDto>();

        CreateMap<Answer, AnswerDto>();

        CreateMap<Choice, ChoiceDto>();

        CreateMap<ChoiceDto, Choice>()
            .Ignore(q => q.Id)
            .Ignore(q => q.TenantId)
            .Ignore(q => q.ChoosableQuestionId);

        CreateMap<QuestionBase, QuestionDto>()
            .ForMember(dest => dest.IsRequired,
                src => src.MapFrom(q => (q as IRequired).IsRequired))
            .ForMember(dest => dest.HasOtherOption,
                src => src.MapFrom(q => (q as IHasOtherOption).HasOtherOption))
            .ForMember(
                dest => dest.Choices,
                src => src.MapFrom(q => (q as IChoosable).GetChoices().OrderBy(t => t.Index)));

        CreateMap<Form, FormSettingsDto>()
            .ForMember(q => q.IsQuiz, src => src.MapFrom(t => t.IsQuiz))
            .ForMember(q => q.CanEditResponse, src => src.MapFrom(t => t.CanEditResponse))
            .ForMember(q => q.IsAcceptingResponses, src => src.MapFrom(t => t.IsAcceptingResponses))
            .ForMember(q => q.IsCollectingEmail, src => src.MapFrom(t => t.IsCollectingEmail))
            .ForMember(q => q.RequiresLogin, src => src.MapFrom(t => t.RequiresLogin))
            .ForMember(q => q.HasLimitOneResponsePerUser,
                src => src.MapFrom(t => t.HasLimitOneResponsePerUser));

        CreateMap<Form, FormDto>()
            .ForMember(dest => dest.IsQuiz, src =>
                src.MapFrom(t => t.IsQuiz))
            .ForMember(dest => dest.IsCollectingEmail, src =>
                src.MapFrom(t => t.IsCollectingEmail))
            .ForMember(dest => dest.RequiresLogin, src =>
                src.MapFrom(t => t.RequiresLogin))
            .ForMember(dest => dest.CanEditResponse, src =>
                src.MapFrom(t => t.CanEditResponse))
            .ForMember(dest => dest.IsAcceptingResponses, src =>
                src.MapFrom(t => t.IsAcceptingResponses))
            .ForMember(dest => dest.HasLimitOneResponsePerUser, src =>
                src.MapFrom(t => t.HasLimitOneResponsePerUser));

        CreateMap<FormWithQuestions, FormWithDetailsDto>()
            .ForMember(dest => dest.Description, src =>
                src.MapFrom(q => q.Form.Description))
            .ForMember(dest => dest.Title, src =>
                src.MapFrom(q => q.Form.Title))
            .ForMember(dest => dest.Id, src =>
                src.MapFrom(q => q.Form.Id))
            .ForMember(dest => dest.CreationTime, src =>
                src.MapFrom(q => q.Form.CreationTime))
            .ForMember(dest => dest.TenantId, src =>
                src.MapFrom(q => q.Form.TenantId))
            .ForMember(dest => dest.DeleterId, src =>
                src.MapFrom(q => q.Form.DeleterId))
            .ForMember(dest => dest.CreatorId, src =>
                src.MapFrom(q => q.Form.CreatorId))
            .ForMember(dest => dest.DeletionTime, src =>
                src.MapFrom(q => q.Form.DeletionTime))
            .ForMember(dest => dest.IsDeleted, src =>
                src.MapFrom(q => q.Form.IsDeleted))
            .ForMember(dest => dest.LastModificationTime, src =>
                src.MapFrom(q => q.Form.LastModificationTime))
            .ForMember(dest => dest.LastModifierId, src =>
                src.MapFrom(q => q.Form.LastModifierId))
            .ForMember(dest => dest.IsQuiz, src =>
                src.MapFrom(t => t.Form.IsQuiz))
            .ForMember(dest => dest.IsCollectingEmail, src =>
                src.MapFrom(t => t.Form.IsCollectingEmail))
            .ForMember(dest => dest.CanEditResponse, src =>
                src.MapFrom(t => t.Form.CanEditResponse))
            .ForMember(dest => dest.IsAcceptingResponses, src =>
                src.MapFrom(t => t.Form.IsAcceptingResponses))
            .ForMember(dest => dest.HasLimitOneResponsePerUser, src =>
                src.MapFrom(t => t.Form.HasLimitOneResponsePerUser))
            .ForMember(dest => dest.RequiresLogin, src =>
                src.MapFrom(t => t.Form.RequiresLogin));

        CreateMap<Form, FormWithAnswersDto>()
            .ForMember(dest => dest.Description, src =>
                src.MapFrom(q => q.Description))
            .ForMember(dest => dest.Title, src =>
                src.MapFrom(q => q.Title))
            .ForMember(dest => dest.Id, src =>
                src.MapFrom(q => q.Id))
            .ForMember(dest => dest.CreationTime, src =>
                src.MapFrom(q => q.CreationTime))
            .ForMember(dest => dest.TenantId, src =>
                src.MapFrom(q => q.TenantId))
            .ForMember(dest => dest.DeleterId, src =>
                src.MapFrom(q => q.DeleterId))
            .ForMember(dest => dest.CreatorId, src =>
                src.MapFrom(q => q.CreatorId))
            .ForMember(dest => dest.DeletionTime, src =>
                src.MapFrom(q => q.DeletionTime))
            .ForMember(dest => dest.IsDeleted, src =>
                src.MapFrom(q => q.IsDeleted))
            .ForMember(dest => dest.LastModificationTime, src =>
                src.MapFrom(q => q.LastModificationTime))
            .ForMember(dest => dest.LastModifierId, src =>
                src.MapFrom(q => q.LastModifierId))
            .ForMember(dest => dest.IsQuiz, src =>
                src.MapFrom(t => t.IsQuiz))
            .ForMember(dest => dest.IsCollectingEmail, src =>
                src.MapFrom(t => t.IsCollectingEmail))
            .ForMember(dest => dest.CanEditResponse, src =>
                src.MapFrom(t => t.CanEditResponse))
            .ForMember(dest => dest.IsAcceptingResponses, src =>
                src.MapFrom(t => t.IsAcceptingResponses))
            .ForMember(dest => dest.HasLimitOneResponsePerUser, src =>
                src.MapFrom(t => t.HasLimitOneResponsePerUser))
            .ForMember(dest => dest.RequiresLogin, src =>
                src.MapFrom(t => t.RequiresLogin))
            .Ignore(q => q.Questions);

        CreateMap<QuestionBase, QuestionWithAnswersDto>()
            .ForMember(dest => dest.IsRequired,
                src => src.MapFrom(q => (q as IRequired).IsRequired))
            .ForMember(dest => dest.HasOtherOption,
                src => src.MapFrom(q => (q as IHasOtherOption).HasOtherOption))
            .ForMember(dest => dest.Choices,
                src => src.MapFrom(q => (q as IChoosable).GetChoices().OrderBy(t => t.Index)))
            .Ignore(q => q.Answers);

        CreateMap<QuestionWithAnswers, QuestionWithAnswersDto>()
            .ForMember(dest => dest.Id,
                src => src.MapFrom(q => q.Question.Id))
            .ForMember(dest => dest.Index,
                src => src.MapFrom(q => q.Question.Index))
            .ForMember(dest => dest.Title,
                src => src.MapFrom(q => q.Question.Title))
            .ForMember(dest => dest.Description,
                src => src.MapFrom(q => q.Question.Description))
            .ForMember(dest => dest.IsRequired,
                src => src.MapFrom(q => (q.Question as IRequired).IsRequired))
            .ForMember(dest => dest.HasOtherOption,
                src => src.MapFrom(q => (q.Question as IHasOtherOption).HasOtherOption))
            .ForMember(dest => dest.QuestionType,
                src => src.MapFrom(q => q.Question.GetQuestionType()))
            .ForMember(dest => dest.FormId,
                src => src.MapFrom(q => q.Question.FormId))
            .IgnoreFullAuditedObjectProperties();

        CreateMap<FormResponse, FormResponseDetailedDto>();
    }
}
