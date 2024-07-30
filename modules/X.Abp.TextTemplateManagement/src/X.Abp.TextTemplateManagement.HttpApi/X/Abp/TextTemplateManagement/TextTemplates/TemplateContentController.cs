// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.TextTemplateManagement.Permissions;

namespace X.Abp.TextTemplateManagement.TextTemplates;

[Route("api/text-template-management/template-contents")]
[Authorize(AbpTextTemplateManagementPermissions.TextTemplates.Default)]
[ControllerName("TextTemplateContents")]
[RemoteService(true, Name = AbpTextTemplateManagementRemoteServiceConsts.RemoteServiceName)]
[Area(AbpTextTemplateManagementRemoteServiceConsts.ModuleName)]
public class TemplateContentController :
  AbpControllerBase,
  ITemplateContentAppService
{
    protected ITemplateContentAppService TemplateContentAppService { get; }

    public TemplateContentController(ITemplateContentAppService templateContentAppService)
    {
        TemplateContentAppService = templateContentAppService;
    }

    [HttpGet]
    public virtual async Task<TextTemplateContentDto> GetAsync(GetTemplateContentInput input)
    {
        return await TemplateContentAppService.GetAsync(input);
    }

    [Route("restore-to-default")]
    [Authorize(AbpTextTemplateManagementPermissions.TextTemplates.EditContents)]
    [HttpPut]
    public virtual async Task RestoreToDefaultAsync(RestoreTemplateContentInput input)
    {
        await TemplateContentAppService.RestoreToDefaultAsync(input);
    }

    [HttpPut]
    public virtual async Task<TextTemplateContentDto> UpdateAsync(UpdateTemplateContentInput input)
    {
        return await TemplateContentAppService.UpdateAsync(input);
    }
}
