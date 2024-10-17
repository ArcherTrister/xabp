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
using Microsoft.Extensions.Caching.Distributed;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.Features;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.Features;
using X.Abp.CmsKit.GlobalFeatures;
using X.Abp.CmsKit.Newsletters;
using X.Abp.CmsKit.Newsletters.Helpers;

namespace X.Abp.CmsKit.Admin.Newsletters;

[Authorize(AbpCmsKitProAdminPermissions.Newsletters.Default)]
[RequiresFeature(CmsKitProFeatures.NewsletterEnable)]
[RequiresGlobalFeature(typeof(NewslettersFeature))]
public class NewsletterRecordAdminAppService : CmsKitProAdminAppServiceBase, INewsletterRecordAdminAppService
{
    protected INewsletterRecordRepository NewsletterRecordsRepository { get; }

    protected NewsletterRecordManager NewsletterRecordManager { get; }

    protected SecurityCodeProvider SecurityCodeProvider { get; }

    protected IDistributedCache<NewsletterDownloadTokenCacheItem, string> DownloadTokenCache { get; }

    protected IDistributedCache<ImportInvalidNewslettersCacheItem, string> ImportInvalidNewslettersCache { get; }

    public NewsletterRecordAdminAppService(
      INewsletterRecordRepository newsletterRecordsRepository,
      NewsletterRecordManager newsletterRecordManager,
      SecurityCodeProvider securityCodeProvider,
      IDistributedCache<NewsletterDownloadTokenCacheItem, string> downloadTokenCache,
      IDistributedCache<ImportInvalidNewslettersCacheItem, string> importInvalidNewslettersCache)
    {
        NewsletterRecordsRepository = newsletterRecordsRepository;
        NewsletterRecordManager = newsletterRecordManager;
        SecurityCodeProvider = securityCodeProvider;
    }

    public virtual async Task<PagedResultDto<NewsletterRecordDto>> GetListAsync(GetNewsletterRecordsRequestInput input)
    {
        var count = await NewsletterRecordsRepository.GetCountByFilterAsync(input.Preference, input.Source);
        var summaryQueryResultItemList = await NewsletterRecordsRepository.GetListAsync(input.Preference, input.Source, input.EmailAddress, input.SkipCount, input.MaxResultCount);
        return new PagedResultDto<NewsletterRecordDto>(count, ObjectMapper.Map<List<NewsletterSummaryQueryResultItem>, List<NewsletterRecordDto>>(summaryQueryResultItemList));
    }

    public virtual async Task<NewsletterRecordWithDetailsDto> GetAsync(Guid id)
    {
        var newsletterRecord = await NewsletterRecordsRepository.GetAsync(id, true);
        return ObjectMapper.Map<NewsletterRecord, NewsletterRecordWithDetailsDto>(newsletterRecord);
    }

    public virtual async Task<List<NewsletterRecordCsvDto>> GetNewsletterRecordsCsvDetailAsync(GetNewsletterRecordsCsvRequestInput input)
    {
        List<NewsletterSummaryQueryResultItem> summaryQueryResultItemList = await NewsletterRecordsRepository.GetListAsync(input.Preference, input.Source, input.EmailAddress);
        List<NewsletterRecordCsvDto> recordsCsvDetails = new List<NewsletterRecordCsvDto>();
        foreach (NewsletterSummaryQueryResultItem summaryQueryResultItem in summaryQueryResultItemList)
        {
            string emailAddress = summaryQueryResultItem.EmailAddress;
            string securityCode = SecurityCodeProvider.GetSecurityCode(emailAddress);
            foreach (string preference in summaryQueryResultItem.Preferences)
            {
                recordsCsvDetails.Add(new NewsletterRecordCsvDto()
                {
                    EmailAddress = emailAddress,
                    SecurityCode = securityCode,
                    Preference = preference
                });
            }
        }

        return recordsCsvDetails;
    }

    public virtual async Task<List<string>> GetNewsletterPreferencesAsync()
    {
        return (await NewsletterRecordManager.GetNewsletterPreferencesAsync()).Select(newsletterPreference => newsletterPreference.Preference).ToList();
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetCsvResponsesAsync(GetNewsletterRecordsCsvRequestInput input)
    {
        await CheckDownloadTokenAsync(input.Token);
        List<NewsletterRecordCsvDto> newsletterRecordCsvDtoList = await GetNewsletterRecordsCsvDetailAsync(input);
        CsvConfiguration csvConfiguration = new CsvConfiguration(new CultureInfo(CultureInfo.CurrentUICulture.Name), null);

        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (StreamWriter streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(true)))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration, false))
                {
                    await csvWriter.WriteRecordsAsync(newsletterRecordCsvDtoList, default);
                    await streamWriter.FlushAsync();
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    MemoryStream ms = new MemoryStream();
                    await memoryStream.CopyToAsync(ms);
                    ms.Seek(0L, SeekOrigin.Begin);
                    return new RemoteStreamContent(ms, $"newsletter-emails-{DateTime.Now:yyyyMMddHHmmss}{input.Preference}{input.Source}.csv", "text/csv");
                }
            }
        }
    }

    public virtual async Task<List<NewsletterPreferenceDetailsDto>> GetNewsletterPreferencesAsync(string emailAddress)
    {
        NewsletterRecord newsletterRecord = await NewsletterRecordsRepository.FindByEmailAddressAsync(emailAddress);
        if (newsletterRecord == null)
        {
            return new List<NewsletterPreferenceDetailsDto>();
        }

        List<NewsletterPreferenceDefinition> preferenceDefinitionList = await NewsletterRecordManager.GetNewsletterPreferencesAsync();
        List<NewsletterPreferenceDetailsDto> preferences = new List<NewsletterPreferenceDetailsDto>();
        foreach (NewsletterPreferenceDefinition preferenceDefinition in preferenceDefinitionList)
        {
            preferences.Add(new NewsletterPreferenceDetailsDto()
            {
                Preference = preferenceDefinition.Preference,
                DisplayPreference = (string)preferenceDefinition.DisplayPreference.Localize(StringLocalizerFactory),
                IsSelectedByEmailAddress = newsletterRecord.Preferences.Any(x => x.Preference == preferenceDefinition.Preference),
                Definition = (string)preferenceDefinition.Definition?.Localize(StringLocalizerFactory)
            });
        }

        return preferences;
    }

    [Authorize(AbpCmsKitProAdminPermissions.Newsletters.EditPreferences)]
    public virtual async Task UpdatePreferencesAsync(UpdatePreferenceInput input)
    {
        NewsletterRecord entity = await NewsletterRecordsRepository.FindByEmailAddressAsync(input.EmailAddress);
        if (entity == null)
        {
            return;
        }

        if (input.PreferenceDetails.Any(x => x.IsEnabled))
        {
            foreach (PreferenceDetailsDto preferenceDetail in input.PreferenceDetails)
            {
                NewsletterPreference newsletterPreference = entity.Preferences.FirstOrDefault(x => x.Preference == preferenceDetail.Preference);
                if (newsletterPreference == null && preferenceDetail.IsEnabled)
                {
                    entity.AddPreferences(new NewsletterPreference(GuidGenerator.Create(), entity.Id, preferenceDetail.Preference, input.Source, input.SourceUrl, CurrentTenant.Id));
                }
                else if (newsletterPreference != null && !preferenceDetail.IsEnabled)
                {
                    entity.RemovePreference(newsletterPreference.Id);
                }
            }

            await NewsletterRecordsRepository.UpdateAsync(entity);
        }
        else
        {
            await NewsletterRecordsRepository.DeleteAsync(entity.Id, false);
        }
    }

    public async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
    {
        string token = Guid.NewGuid().ToString("n");
        NewsletterDownloadTokenCacheItem downloadTokenCacheItem = new NewsletterDownloadTokenCacheItem
        {
            Token = token,
            TenantId = CurrentTenant.Id
        };
        DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30.0)
        };
        await DownloadTokenCache.SetAsync(token, downloadTokenCacheItem, options);
        return new DownloadTokenResultDto()
        {
            Token = token
        };
    }

    [AllowAnonymous]
    public virtual async Task<IRemoteStreamContent> GetImportNewslettersSampleFileAsync(GetImportNewslettersSampleFileInput input)
    {
        await CheckDownloadTokenAsync(input.Token);
        return await GetImportNewslettersFileAsync(new List<ImportNewslettersFromFileDto>()
        {
            new ImportNewslettersFromFileDto()
            {
              EmailAddress = "zhangsan@abp.com",
              Preference = "News",
              Source = "baidu",
              SourceUrl = "https://www.baidu.com"
            },
            new ImportNewslettersFromFileDto()
            {
              EmailAddress = "lisi@abp.com",
              Preference = "News",
              Source = "qq",
              SourceUrl = "https://www.qq.com"
            }
        },
        "ImportNewslettersSampleFile");
    }

    [Authorize(AbpCmsKitProAdminPermissions.Newsletters.Import)]
    public async Task<ImportNewslettersFromFileOutput> ImportNewslettersFromFileAsync(ImportNewslettersFromFileInputWithStream input)
    {
        MemoryStream stream = new MemoryStream();
        await input.File.GetStream().CopyToAsync(stream);
        List<InvalidImportNewslettersFromFileDto> invalidNewsletters = new List<InvalidImportNewslettersFromFileDto>();
        List<ImportNewslettersFromFileDto> list;
        try
        {
            stream.Seek(0L, SeekOrigin.Begin);
            using (StreamReader streamReader = new StreamReader(stream))
            {
                using (CsvReader csvReader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture, null), false))
                {
                    // list = csvReader.GetRecords<ImportNewslettersFromFileDto>().ToList();
                    list = await csvReader.GetRecordsAsync<ImportNewslettersFromFileDto>(default).ToListAsync();
                }
            }
        }
        catch (Exception)
        {
            throw new UserFriendlyException(L["InvalidFileFormat"]);
        }

        ImportNewslettersFromFileOutput resultDto = list.Count != 0 ? new ImportNewslettersFromFileOutput()
        {
            AllCount = list.Count
        }
        : throw new UserFriendlyException(L["NoNewsletterFoundInTheFile"]);

        foreach (IGrouping<string, ImportNewslettersFromFileDto> grouping in list.GroupBy(x => x.EmailAddress))
        {
            IGrouping<string, ImportNewslettersFromFileDto> waitingImportNewsletter = grouping;
            if (string.IsNullOrWhiteSpace(waitingImportNewsletter.Key))
            {
                invalidNewsletters.AddRange(waitingImportNewsletter.Select(x =>
                {
                    return new InvalidImportNewslettersFromFileDto()
                    {
                        EmailAddress = x.EmailAddress,
                        Preference = x.Preference
                    };
                }));
            }
            else
            {
                foreach (ImportNewslettersFromFileDto newslettersFromFileDto in (IEnumerable<ImportNewslettersFromFileDto>)waitingImportNewsletter)
                {
                    if (string.IsNullOrWhiteSpace(newslettersFromFileDto.Preference))
                    {
                        invalidNewsletters.Add(new InvalidImportNewslettersFromFileDto
                        {
                            EmailAddress = newslettersFromFileDto.EmailAddress,
                            Preference = newslettersFromFileDto.Preference
                        });
                    }
                }

                foreach (ImportNewslettersFromFileDto preference in waitingImportNewsletter.DistinctBy(x => x.Preference).Where(x => !string.IsNullOrWhiteSpace(x.Preference)).ToList())
                {
                    try
                    {
                        await NewsletterRecordManager.CreateOrUpdateAsync(waitingImportNewsletter.Key, preference.Preference, preference.Source, preference.SourceUrl, null);
                    }
                    catch (Exception ex)
                    {
                        InvalidImportNewslettersFromFileDto newslettersFromFileDto = new InvalidImportNewslettersFromFileDto
                        {
                            EmailAddress = preference.EmailAddress,
                            Preference = preference.Preference,
                            Source = preference.Source,
                            SourceUrl = preference.SourceUrl,
                            ErrorReason = ex.Message
                        };
                        invalidNewsletters.Add(newslettersFromFileDto);
                    }
                }
            }
        }

        if (invalidNewsletters.Count != 0)
        {
            string token = Guid.NewGuid().ToString("n");
            ImportInvalidNewslettersCacheItem newslettersCacheItem = new ImportInvalidNewslettersCacheItem
            {
                Token = token,
                InvalidNewsletters = invalidNewsletters
            };
            DistributedCacheEntryOptions options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1.0)
            };

            await ImportInvalidNewslettersCache.SetAsync(token, newslettersCacheItem, options);
            resultDto.InvalidNewslettersDownloadToken = token;
        }

        resultDto.SucceededCount = resultDto.AllCount - invalidNewsletters.Count;
        resultDto.FailedCount = invalidNewsletters.Count;
        return resultDto;
    }

    [AllowAnonymous]
    public async Task<IRemoteStreamContent> GetImportInvalidNewslettersFileAsync(GetImportInvalidNewslettersFileInput input)
    {
        await CheckDownloadTokenAsync(input.Token, true);
        return await GetImportNewslettersFileAsync((await ImportInvalidNewslettersCache.GetAsync(input.Token)).InvalidNewsletters, "InvalidNewsletters");
    }

    protected virtual async Task<IRemoteStreamContent> GetImportNewslettersFileAsync<T>(List<T> newsletters, string fileName)
        where T : ImportNewslettersFromFileDto
    {
        CsvConfiguration csvConfiguration = new CsvConfiguration(new CultureInfo(CultureInfo.CurrentUICulture.Name), null);
        IRemoteStreamContent newslettersFileAsync;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (StreamWriter streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(true)))
            {
                using (CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration, false))
                {
                    await csvWriter.WriteRecordsAsync(newsletters, default);
                    await streamWriter.FlushAsync();
                    memoryStream.Seek(0L, SeekOrigin.Begin);
                    MemoryStream ms = new MemoryStream();
                    await memoryStream.CopyToAsync(ms);
                    ms.Seek(0L, SeekOrigin.Begin);
                    newslettersFileAsync = new RemoteStreamContent(ms, fileName + ".csv", "text/csv");
                }
            }
        }

        return newslettersFileAsync;
    }

    protected virtual async Task CheckDownloadTokenAsync(string token, bool isInvalidNewslettersToken = false)
    {
        IDownloadCacheItem downloadCacheItem;
        if (!isInvalidNewslettersToken)
        {
            downloadCacheItem = await DownloadTokenCache.GetAsync(token);
        }
        else
        {
            downloadCacheItem = await ImportInvalidNewslettersCache.GetAsync(token);
        }

        if (downloadCacheItem == null || token != downloadCacheItem.Token)
        {
            throw new AbpAuthorizationException("Invalid download token: " + token);
        }
    }
}
