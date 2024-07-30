// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace X.Abp.TextTemplateManagement.TextTemplates;

public interface ITemplateContentAppService : IApplicationService
{
    Task<TextTemplateContentDto> GetAsync(GetTemplateContentInput input);

    Task RestoreToDefaultAsync(RestoreTemplateContentInput input);

    Task<TextTemplateContentDto> UpdateAsync(
      UpdateTemplateContentInput input);
}
