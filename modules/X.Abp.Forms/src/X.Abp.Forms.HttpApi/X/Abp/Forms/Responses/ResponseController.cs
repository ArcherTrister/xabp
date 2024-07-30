// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Forms.Forms;
using X.Abp.Forms.Questions;

namespace X.Abp.Forms.Responses;

[RemoteService(Name = AbpFormsRemoteServiceConsts.RemoteServiceName)]
[Area(AbpFormsRemoteServiceConsts.ModuleName)]
[ControllerName("Form")]
[Route("api/responses")]
public class ResponseController : AbpControllerBase, IResponseAppService
{
    protected IResponseAppService ResponseAppService { get; }

    public ResponseController(IResponseAppService responseAppService)
    {
        ResponseAppService = responseAppService;
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<FormResponseDto> GetAsync(Guid id)
    {
        return ResponseAppService.GetAsync(id);
    }

    [HttpGet]
    public virtual Task<PagedResultDto<FormResponseDto>> GetListAsync(GetUserFormListInputDto input)
    {
        return ResponseAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}/questions-with-answers")]
    public Task<List<QuestionWithAnswersDto>> GetQuestionsWithAnswersAsync(Guid id)
    {
        return ResponseAppService.GetQuestionsWithAnswersAsync(id);
    }

    [HttpGet]
    [Route("form-details/{formId}")]
    public virtual Task<FormDto> GetFormDetailsAsync(Guid formId)
    {
        return ResponseAppService.GetFormDetailsAsync(formId);
    }

    [HttpGet]
    [Route("{userId}/response")]
    public virtual Task<PagedResultDto<FormWithResponseDto>> GetUserFormsByUserIdAsync(Guid userId)
    {
        return ResponseAppService.GetUserFormsByUserIdAsync(userId);
    }

    [HttpPost]
    public virtual Task<FormResponseDto> SaveAnswersAsync(Guid formId, CreateResponseDto input)
    {
        return ResponseAppService.SaveAnswersAsync(formId, input);
    }

    [HttpPost]
    [Route("{id}")]
    public virtual Task<FormResponseDto> UpdateAnswersAsync(Guid id, UpdateResponseDto input)
    {
        return ResponseAppService.UpdateAnswersAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return ResponseAppService.DeleteAsync(id);
    }
}
