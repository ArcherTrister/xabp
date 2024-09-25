// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CsvHelper;
using CsvHelper.Configuration;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.Newsletters.Helpers;

namespace X.Abp.CmsKit.Admin.Newsletters;

[Authorize(AbpCmsKitProAdminPermissions.Newsletters.Default)]
public class NewsletterRecordAdminAppService : CmsKitProAdminAppServiceBase, INewsletterRecordAdminAppService
{
  protected INewsletterRecordRepository NewsletterRecordsRepository { get; }

  protected NewsletterRecordManager NewsletterRecordManager { get; }

  protected SecurityCodeProvider SecurityCodeProvider { get; }

  public NewsletterRecordAdminAppService(
    INewsletterRecordRepository newsletterRecordsRepository,
    NewsletterRecordManager newsletterRecordManager,
    SecurityCodeProvider securityCodeProvider)
  {
    NewsletterRecordsRepository = newsletterRecordsRepository;
    NewsletterRecordManager = newsletterRecordManager;
    SecurityCodeProvider = securityCodeProvider;
  }

  public virtual async Task<PagedResultDto<NewsletterRecordDto>> GetListAsync(GetNewsletterRecordsRequestInput input)
  {
    var count = await NewsletterRecordsRepository.GetCountByFilterAsync(input.Preference, input.Source);
    var summaryQueryResultItemList = await NewsletterRecordsRepository.GetListAsync(input.Preference, input.Source, input.SkipCount, input.MaxResultCount);
    return new PagedResultDto<NewsletterRecordDto>(count, ObjectMapper.Map<List<NewsletterSummaryQueryResultItem>, List<NewsletterRecordDto>>(summaryQueryResultItemList));
  }

  public virtual async Task<NewsletterRecordWithDetailsDto> GetAsync(
    Guid id)
  {
    var newsletterRecord = await NewsletterRecordsRepository.GetAsync(id, true);
    return ObjectMapper.Map<NewsletterRecord, NewsletterRecordWithDetailsDto>(newsletterRecord);
  }

  public virtual async Task<List<NewsletterRecordCsvDto>> GetNewsletterRecordsCsvDetailAsync(GetNewsletterRecordsCsvRequestInput input)
  {
    var newsletterSummaryQueryResultItems = await NewsletterRecordsRepository.GetListAsync(input.Preference, input.Source);
    var newsletterRecords = ObjectMapper.Map<List<NewsletterSummaryQueryResultItem>, List<NewsletterRecordCsvDto>>(newsletterSummaryQueryResultItems);
    foreach (var newsletterRecordCsvDto in newsletterRecords)
    {
      newsletterRecordCsvDto.SecurityCode = SecurityCodeProvider.GetSecurityCode(newsletterRecordCsvDto.EmailAddress);
    }

    return newsletterRecords;
  }

  public virtual async Task<List<string>> GetNewsletterPreferencesAsync()
  {
    return (await NewsletterRecordManager.GetNewsletterPreferencesAsync()).Select(newsletterPreference => newsletterPreference.Preference).ToList();
  }

  public virtual async Task<IRemoteStreamContent> GetCsvResponsesAsync(GetNewsletterRecordsCsvRequestInput input)
  {
    var newsletterRecordCsvDtoList = await GetNewsletterRecordsCsvDetailAsync(input);
    input.Preference = input.Preference?.Insert(0, "-");
    input.Source = input.Source?.Insert(0, "-");
    var csvConfiguration = new CsvConfiguration(new CultureInfo(CultureInfo.CurrentUICulture.Name));

    using var memoryStream = new MemoryStream();
    using var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(true));
    using var csvWriter = new CsvWriter(streamWriter, csvConfiguration);
    await csvWriter.WriteRecordsAsync(newsletterRecordCsvDtoList);

    await streamWriter.FlushAsync();
    memoryStream.Seek(0L, SeekOrigin.Begin);
    var ms = new MemoryStream();
    await memoryStream.CopyToAsync(ms);

    ms.Seek(0L, SeekOrigin.Begin);
    return new RemoteStreamContent(ms, $"newsletter-emails-{DateTime.Now:yyyyMMddHHmmss}{input.Preference}{input.Source}.csv", "text/csv");
  }
}
