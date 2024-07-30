// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Features;
using Volo.Abp.Users;

using X.Abp.Forms.Forms;
using X.Abp.Forms.Permissions;
using X.Abp.Forms.Questions;

namespace X.Abp.Forms.Responses;

[RequiresFeature(FormsFeatures.Enable)]
public class ResponseAppService : FormsAppServiceBase, IResponseAppService
{
    protected IResponseRepository ResponseRepository { get; }

    protected IFormRepository FormRepository { get; }

    protected IQuestionRepository QuestionRepository { get; }

    public ResponseAppService(
        IResponseRepository responseRepository,
        IFormRepository formRepository,
        IQuestionRepository questionRepository)
    {
        ResponseRepository = responseRepository;
        FormRepository = formRepository;
        QuestionRepository = questionRepository;
    }

    public virtual async Task<FormResponseDto> GetAsync(Guid id)
    {
        var formResponse = await ResponseRepository.GetAsync(id);

        return ObjectMapper.Map<FormResponse, FormResponseDto>(formResponse);
    }

    public virtual async Task<List<QuestionWithAnswersDto>> GetQuestionsWithAnswersAsync(Guid id)
    {
        var questions = await QuestionRepository.GetListWithAnswersByFormResponseId(id);

        return ObjectMapper.Map<List<QuestionWithAnswers>, List<QuestionWithAnswersDto>>(questions);
    }

    public virtual async Task<PagedResultDto<FormResponseDto>> GetListAsync(GetUserFormListInputDto input)
    {
        var userForms = await ResponseRepository.GetListAsync(
            input.SkipCount,
            input.MaxResultCount,
            input.Sorting,
            input.Filter);

        return new PagedResultDto<FormResponseDto>(
            userForms.Count,
            ObjectMapper.Map<List<FormResponse>, List<FormResponseDto>>(userForms));
    }

    public virtual async Task<FormDto> GetFormDetailsAsync(Guid formId)
    {
        var form = await FormRepository.GetAsync(formId);

        return ObjectMapper.Map<Form, FormDto>(form);
    }

    public virtual async Task<PagedResultDto<FormWithResponseDto>> GetUserFormsByUserIdAsync(Guid userId)
    {
        var userForms = await ResponseRepository.GetByUserId(userId);

        var dto = ObjectMapper.Map<List<FormWithResponse>, List<FormWithResponseDto>>(userForms);
        return new PagedResultDto<FormWithResponseDto>(dto.Count, dto);
    }

    public virtual async Task<FormResponseDto> SaveAnswersAsync(Guid formId, CreateResponseDto input)
    {
        var form = await FormRepository.GetAsync(formId);

        if (form.RequiresLogin)
        {
            if (!CurrentUser.IsAuthenticated)
            {
                throw new AbpAuthorizationException();
            }

            if (form.HasLimitOneResponsePerUser && await ResponseRepository.UserResponseExistsAsync(formId, CurrentUser.GetId()))
            {
                throw new UserResponseAlreadyExistException();
            }
        }

        if (form.IsCollectingEmail && string.IsNullOrWhiteSpace(input.Email))
        {
            throw new EmailAddressRequiredException();
        }

        var newResponse = new FormResponse(GuidGenerator.Create(), form.Id, CurrentUser.Id, input.Email);

        foreach (var answerDto in input.Answers)
        {
            newResponse.AddOrUpdateAnswer(answerDto.QuestionId, GuidGenerator.Create(), answerDto.ChoiceId, answerDto.Value);
        }

        var response = await ResponseRepository.InsertAsync(newResponse);

        return ObjectMapper.Map<FormResponse, FormResponseDto>(response);
    }

    public virtual async Task<FormResponseDto> UpdateAnswersAsync(Guid id, UpdateResponseDto input)
    {
        var form = await FormRepository.GetAsync(input.FormId);

        var formResponse = await ResponseRepository.GetAsync(id);

        if (!form.CanEditResponse)
        {
            throw new ResponseNotEditableException();
        }

        if (form.RequiresLogin && CurrentUser.Id != formResponse.UserId)
        {
            throw new AbpAuthorizationException();
        }

        if (form.IsCollectingEmail && string.IsNullOrWhiteSpace(input.Email))
        {
            throw new EmailAddressRequiredException();
        }

        formResponse.SetEmail(input.Email);

        // Remove - Re-Add
        formResponse.Answers.Clear();
        foreach (var answer in input.Answers)
        {
            formResponse.AddOrUpdateAnswer(answer.QuestionId, GuidGenerator.Create(), answer.ChoiceId, answer.Value);
        }

        var updatedResponse = await ResponseRepository.UpdateAsync(formResponse);

        return ObjectMapper.Map<FormResponse, FormResponseDto>(updatedResponse);
    }

    [Authorize(AbpFormsPermissions.Response.Delete)]
    public virtual Task DeleteAsync(Guid id)
    {
        return ResponseRepository.DeleteAsync(id);
    }
}
