// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace X.Abp.CmsKit.Admin.Newsletters;

public interface INewsletterRecordAdminAppService : IApplicationService
{
    Task<PagedResultDto<NewsletterRecordDto>> GetListAsync(GetNewsletterRecordsRequestInput input);

    Task<NewsletterRecordWithDetailsDto> GetAsync(Guid id);

    Task<List<NewsletterRecordCsvDto>> GetNewsletterRecordsCsvDetailAsync(GetNewsletterRecordsCsvRequestInput input);

    Task<List<string>> GetNewsletterPreferencesAsync();

    Task<IRemoteStreamContent> GetCsvResponsesAsync(GetNewsletterRecordsCsvRequestInput input);

    Task<List<NewsletterPreferenceDetailsDto>> GetNewsletterPreferencesAsync(string emailAddress);

    Task UpdatePreferencesAsync(UpdatePreferenceInput input);

    Task<DownloadTokenResultDto> GetDownloadTokenAsync();

    Task<IRemoteStreamContent> GetImportNewslettersSampleFileAsync(GetImportNewslettersSampleFileInput input);

    Task<ImportNewslettersFromFileOutput> ImportNewslettersFromFileAsync(ImportNewslettersFromFileInputWithStream input);

    Task<IRemoteStreamContent> GetImportInvalidNewslettersFileAsync(GetImportInvalidNewslettersFileInput input);
}
