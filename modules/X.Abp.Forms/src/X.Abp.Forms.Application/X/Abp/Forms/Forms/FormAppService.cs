// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Content;
using Volo.Abp.Data;
using Volo.Abp.Emailing;
using Volo.Abp.Features;

using X.Abp.Forms.Permissions;
using X.Abp.Forms.Questions;
using X.Abp.Forms.Responses;

namespace X.Abp.Forms.Forms;

[RequiresFeature(FormsFeatures.Enable)]
[Authorize(AbpFormsPermissions.Forms.Default)]
public class FormAppService : FormsAppServiceBase, IFormAppService
{
  protected IFormRepository FormRepository { get; }

  protected IQuestionRepository QuestionRepository { get; }

  protected IResponseRepository ResponseRepository { get; }

  protected QuestionManager QuestionManager { get; }

  protected IEmailSender EmailSender { get; }

  public FormAppService(
      QuestionManager questionManager,
      IFormRepository formRepository,
      IQuestionRepository questionRepository,
      IResponseRepository responseRepository,
      IEmailSender emailSender)
  {
    FormRepository = formRepository;
    QuestionRepository = questionRepository;
    QuestionManager = questionManager;
    ResponseRepository = responseRepository;
    EmailSender = emailSender;
  }

  public virtual async Task<PagedResultDto<FormDto>> GetListAsync(GetFormListInputDto input)
  {
    var list = await FormRepository.GetListAsync(input.Sorting, input.MaxResultCount, input.SkipCount, input.Filter);

    var totalCount = await FormRepository.GetCountAsync(input.Filter);

    return new PagedResultDto<FormDto>(totalCount, ObjectMapper.Map<List<Form>, List<FormDto>>(list));
  }

  public virtual async Task<PagedResultDto<FormResponseDetailedDto>> GetResponsesAsync(Guid id, GetResponseListInputDto input)
  {
    var responses = await ResponseRepository.GetListByFormIdAsync(
        id,
        input.SkipCount,
        input.MaxResultCount,
        input.Sorting,
        input.Filter);

    var totalCount = await ResponseRepository.GetCountByFormIdAsync(id, input.Filter);

    return new PagedResultDto<FormResponseDetailedDto>(totalCount, ObjectMapper.Map<List<FormResponse>, List<FormResponseDetailedDto>>(responses));
  }

  public virtual async Task<IRemoteStreamContent> GetCsvResponsesAsync(Guid id, GetResponseListInputDto input)
  {
    var responses = await ResponseRepository.GetListByFormIdAsync(
        id,
        input.SkipCount,
        input.MaxResultCount,
        input.Sorting,
        input.Filter);

    var form = await FormRepository.GetWithQuestionsAsync(id, true);
    var questions = form.Questions.OrderBy(q => q.Index).ToList();

    var headers = questions.Select(q => q.Title).ToList();
    headers.AddFirst("Date");

    var csvConfiguration = new CsvConfiguration(new CultureInfo(CultureInfo.CurrentUICulture.Name))
    {
      ShouldQuote = (field, context) => true
    };

    using var memoryStream = new MemoryStream();
    using var streamWriter = new StreamWriter(stream: memoryStream, encoding: new UTF8Encoding(true));
    using var csvWriter = new CsvWriter(streamWriter, csvConfiguration);
    foreach (var header in headers)
    {
      csvWriter.WriteField(header);
    }

    foreach (var response in responses)
    {
      await csvWriter.NextRecordAsync();

      var date = response.LastModificationTime ?? response.CreationTime;

      csvWriter.WriteField(date.ToString("yyyy-MM-dd HH:mm:ss"));

      foreach (var question in questions)
      {
        var questionResponse =
            response.Answers.FirstOrDefault(x => x.QuestionId == question.Id);

        csvWriter.WriteField(questionResponse?.Value ?? string.Empty);
      }
    }

    await memoryStream.FlushAsync();
    memoryStream.Position = 0;

    var csv = new MemoryStream();
    await memoryStream.CopyToAsync(csv);
    csv.Position = 0;
    return new RemoteStreamContent(csv, $"{form.Form.Title}.csv", "text/csv");
  }

  public virtual async Task<long> GetResponsesCountAsync(Guid id)
  {
    return await ResponseRepository.GetCountByFormIdAsync(id);
  }

  public virtual async Task<FormDto> CreateAsync(CreateFormDto input)
  {
    var form = new Form(GuidGenerator.Create(), input.Title, input.Description, false, false, false, true, false, false, CurrentTenant.Id);

    await FormRepository.InsertAsync(form);

    return ObjectMapper.Map<Form, FormDto>(form);
  }

  [AllowAnonymous]
  public virtual async Task<FormWithDetailsDto> GetAsync(Guid id)
  {
    var result = await FormRepository.GetWithQuestionsAsync(id, true);

    return ObjectMapper.Map<FormWithQuestions, FormWithDetailsDto>(result);
  }

  public virtual async Task<FormDto> UpdateAsync(Guid id, UpdateFormDto input)
  {
    var form = await FormRepository.GetAsync(id);

    form.SetTitle(input.Title);
    form.SetDescription(input.Description);
    form.SetConcurrencyStampIfNotNull(input.ConcurrencyStamp);

    await FormRepository.UpdateAsync(form);

    return ObjectMapper.Map<Form, FormDto>(form);
  }

  public virtual async Task SetSettingsAsync(Guid id, UpdateFormSettingInputDto input)
  {
    var form = await FormRepository.GetAsync(id);

    form.SetSettings(
        input.CanEditResponse,
        input.IsCollectingEmail,
        input.HasLimitOneResponsePerUser,
        input.IsAcceptingResponses,
        input.IsQuiz,
        input.RequiresLogin);

    await FormRepository.UpdateAsync(form);
  }

  public virtual async Task<FormSettingsDto> GetSettingsAsync(Guid formId)
  {
    var form = await FormRepository.GetAsync(formId);

    return new FormSettingsDto
    {
      CanEditResponse = form.CanEditResponse,
      HasLimitOneResponsePerUser = form.HasLimitOneResponsePerUser,
      IsAcceptingResponses = form.IsAcceptingResponses,
      IsCollectingEmail = form.IsCollectingEmail,
      IsQuiz = form.IsQuiz,
      RequiresLogin = form.RequiresLogin
    };
  }

  [AllowAnonymous]
  public virtual async Task<List<QuestionDto>> GetQuestionsAsync(Guid id, GetQuestionListDto input)
  {
    var form = await FormRepository.GetAsync(id);

    if (form.RequiresLogin && !CurrentUser.IsAuthenticated)
    {
      throw new AbpAuthorizationException();
    }

    var items = await QuestionRepository.GetListByFormIdAsync(form.Id);

    return ObjectMapper.Map<List<QuestionBase>, List<QuestionDto>>(items);
  }

  public virtual async Task<QuestionDto> CreateQuestionAsync(Guid id, CreateQuestionDto input)
  {
    var form = await FormRepository.GetAsync(id);

    var choiceList = new List<(string, bool)>();

    foreach (var choice in input.Choices)
    {
      choiceList.Add((choice.Value, choice.IsCorrect));
    }

    return ObjectMapper.Map<QuestionBase, QuestionDto>(
        await QuestionManager.CreateQuestionAsync(
            form,
            input.QuestionType,
            input.Index,
            input.IsRequired,
            input.Title,
            input.Description,
            input.HasOtherOption,
            choiceList));
  }

  [Authorize(AbpFormsPermissions.Forms.Delete)]
  public virtual Task DeleteAsync(Guid id)
  {
    return FormRepository.DeleteAsync(id);
  }

  [Authorize(AbpFormsPermissions.Response.Delete)]
  public virtual async Task DeleteAllResponsesOfFormAsync(Guid id)
  {
    var formResponseIds = (await ResponseRepository.GetListByFormIdAsync(id)).Select(q => q.Id);

    foreach (var formResponseId in formResponseIds)
    {
      await ResponseRepository.DeleteAsync(formResponseId);
    }
  }

  public virtual async Task SendInviteEmailAsync(FormInviteEmailInputDto input)
  {
    await EmailSender.SendAsync(input.To, input.Subject, input.Body);
  }
}
