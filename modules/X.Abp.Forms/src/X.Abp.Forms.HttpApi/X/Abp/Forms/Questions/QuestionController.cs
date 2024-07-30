// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Forms.Localization;

namespace X.Abp.Forms.Questions;

[RemoteService(Name = AbpFormsRemoteServiceConsts.RemoteServiceName)]
[Area(AbpFormsRemoteServiceConsts.ModuleName)]
[ControllerName("Form")]
[Route("api/questions")]
public class QuestionController : AbpControllerBase, IQuestionAppService
{
    protected IQuestionAppService QuestionAppService { get; }

    public QuestionController(IQuestionAppService questionAppService)
    {
        QuestionAppService = questionAppService;
        LocalizationResource = typeof(FormsResource);
    }

    [HttpGet]
    public virtual Task<List<QuestionDto>> GetListAsync(GetQuestionListDto input)
    {
        return QuestionAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{id}")]
    public virtual Task<QuestionDto> GetAsync(Guid id)
    {
        return QuestionAppService.GetAsync(id);
    }

    [HttpPut]
    [Route("{id}")]
    public virtual Task<QuestionDto> UpdateAsync(Guid id, UpdateQuestionDto input)
    {
        return QuestionAppService.UpdateAsync(id, input);
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual Task DeleteAsync(Guid id)
    {
        return QuestionAppService.DeleteAsync(id);
    }
}
