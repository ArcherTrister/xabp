// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.Forms.Forms;
using X.Abp.Forms.Questions;

namespace X.Abp.Forms.Responses;

public interface IResponseAppService : IApplicationService
{
  Task<FormResponseDto> GetAsync(Guid id);

  Task<List<QuestionWithAnswersDto>> GetQuestionsWithAnswersAsync(Guid id);

  Task<PagedResultDto<FormResponseDto>> GetListAsync(GetUserFormListInputDto input);

  Task<FormDto> GetFormDetailsAsync(Guid formId);

  Task<PagedResultDto<FormWithResponseDto>> GetUserFormsByUserIdAsync(Guid userId);

  Task<FormResponseDto> SaveAnswersAsync(Guid formId, CreateResponseDto input);

  Task<FormResponseDto> UpdateAnswersAsync(Guid id, UpdateResponseDto input);

  Task DeleteAsync(Guid id);
}
