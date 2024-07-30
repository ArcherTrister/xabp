// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

using X.Abp.LanguageManagement.Dto;

namespace X.Abp.LanguageManagement;

public interface ILanguageAppService : ICrudAppService<LanguageDto, Guid, GetLanguagesTextsInput, CreateLanguageDto, UpdateLanguageDto>
{
    Task<ListResultDto<LanguageDto>> GetAllListAsync();

    Task SetAsDefaultAsync(Guid id);

    Task<List<LanguageResourceDto>> GetResourcesAsync();

    Task<List<CultureInfoDto>> GetCulturelistAsync();

    Task<List<string>> GetFlagListAsync();
}
