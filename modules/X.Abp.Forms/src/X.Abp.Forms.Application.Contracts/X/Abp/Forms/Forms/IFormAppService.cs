// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

using X.Abp.Forms.Questions;
using X.Abp.Forms.Responses;

namespace X.Abp.Forms.Forms;

public interface IFormAppService : IApplicationService
{
    Task<PagedResultDto<FormDto>> GetListAsync(GetFormListInputDto input);

    Task<PagedResultDto<FormResponseDetailedDto>> GetResponsesAsync(Guid id, GetResponseListInputDto input);

    Task<IRemoteStreamContent> GetCsvResponsesAsync(Guid id, GetResponseListInputDto input);

    Task<long> GetResponsesCountAsync(Guid id);

    Task<FormDto> CreateAsync(CreateFormDto input);

    Task<FormWithDetailsDto> GetAsync(Guid id);

    Task<FormDto> UpdateAsync(Guid id, UpdateFormDto input);

    Task SetSettingsAsync(Guid id, UpdateFormSettingInputDto input);

    Task<FormSettingsDto> GetSettingsAsync(Guid formId);

    Task<List<QuestionDto>> GetQuestionsAsync(Guid id, GetQuestionListDto input);

    Task<QuestionDto> CreateQuestionAsync(Guid id, CreateQuestionDto input);

    Task DeleteAsync(Guid id);

    Task DeleteAllResponsesOfFormAsync(Guid id);

    Task SendInviteEmailAsync(FormInviteEmailInputDto input);
}
