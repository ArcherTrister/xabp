// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Services;

namespace X.Abp.CmsKit.Public.Newsletters;

public interface INewsletterRecordPublicAppService : IApplicationService
{
    Task CreateAsync(CreateNewsletterRecordInput input);

    Task<List<NewsletterPreferenceDetailsDto>> GetNewsletterPreferencesAsync(
      string emailAddress);

    Task UpdatePreferencesAsync(UpdatePreferenceRequestInput input);

    Task<NewsletterEmailOptionsDto> GetOptionByPreferenceAsync(
      string preference);
}
