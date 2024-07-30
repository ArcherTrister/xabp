// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

using X.Abp.LanguageManagement.Dto;

namespace X.Abp.LanguageManagement;

[RemoteService(Name = AbpLanguageManagementRemoteServiceConsts.RemoteServiceName)]
[Area(AbpLanguageManagementRemoteServiceConsts.ModuleName)]
[ControllerName("LanguageTexts")]
[Route("api/language-management/language-texts")]
public class LanguageTextController : AbpControllerBase, ILanguageTextAppService
{
    protected ILanguageTextAppService LanguageTextAppService { get; }

    public LanguageTextController(ILanguageTextAppService languageTextAppService)
    {
        LanguageTextAppService = languageTextAppService;
    }

    [HttpGet]
    public virtual Task<PagedResultDto<LanguageTextDto>> GetListAsync(GetLanguagesTextsInput input)
    {
        return LanguageTextAppService.GetListAsync(input);
    }

    [HttpGet]
    [Route("{resourceName}/{cultureName}/{name}")]
    public virtual Task<LanguageTextDto> GetAsync(string resourceName, string cultureName, string name, string baseCultureName)
    {
        return LanguageTextAppService.GetAsync(resourceName, cultureName, name, baseCultureName);
    }

    [HttpPut]
    [Route("{resourceName}/{cultureName}/{name}")]
    public virtual async Task UpdateAsync(string resourceName, string cultureName, string name, string value)
    {
        await LanguageTextAppService.UpdateAsync(resourceName, cultureName, name, value);
    }

    [HttpPut]
    [Route("{resourceName}/{cultureName}/{name}/restore")]
    public virtual async Task RestoreToDefaultAsync(string resourceName, string cultureName, string name)
    {
        await LanguageTextAppService.RestoreToDefaultAsync(resourceName, cultureName, name);
    }
}
