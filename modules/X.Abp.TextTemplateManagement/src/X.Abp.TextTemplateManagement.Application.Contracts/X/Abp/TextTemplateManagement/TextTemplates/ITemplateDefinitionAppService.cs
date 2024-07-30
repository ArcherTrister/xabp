// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public interface ITemplateDefinitionAppService : IApplicationService
{
    Task<PagedResultDto<TemplateDefinitionDto>> GetListAsync(
      GetTemplateDefinitionListInput input);

    Task<TemplateDefinitionDto> GetAsync(string name);
}
