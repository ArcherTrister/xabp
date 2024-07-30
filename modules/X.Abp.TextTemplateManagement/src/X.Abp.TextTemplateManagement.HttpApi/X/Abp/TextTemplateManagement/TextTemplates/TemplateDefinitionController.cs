// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.TextTemplateManagement.Permissions;

namespace X.Abp.TextTemplateManagement.TextTemplates;

[Route("api/text-template-management/template-definitions")]
[RemoteService(true, Name = AbpTextTemplateManagementRemoteServiceConsts.RemoteServiceName)]
[ControllerName("TextTemplateDefinitions")]
[Authorize(AbpTextTemplateManagementPermissions.TextTemplates.Default)]
[Area(AbpTextTemplateManagementRemoteServiceConsts.ModuleName)]
public class TemplateDefinitionController : AbpControllerBase, ITemplateDefinitionAppService
{
    protected ITemplateDefinitionAppService TemplateDefinitionAppService { get; }

    public TemplateDefinitionController(ITemplateDefinitionAppService templateDefinitionAppService)
    {
        TemplateDefinitionAppService = templateDefinitionAppService;
    }

    [HttpGet]
    public virtual async Task<PagedResultDto<TemplateDefinitionDto>> GetListAsync(GetTemplateDefinitionListInput input)
    {
        return await TemplateDefinitionAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{name}")]
    public virtual async Task<TemplateDefinitionDto> GetAsync(string name)
    {
        return await TemplateDefinitionAppService.GetAsync(name);
    }
}
