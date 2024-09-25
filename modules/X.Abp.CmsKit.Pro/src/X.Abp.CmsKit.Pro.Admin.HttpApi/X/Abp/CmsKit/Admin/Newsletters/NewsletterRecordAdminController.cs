// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;
using Volo.Abp.GlobalFeatures;
using Volo.CmsKit.Admin;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Admin.Newsletters;

[Authorize(AbpCmsKitProAdminPermissions.Newsletters.Default)]
[RequiresGlobalFeature(typeof(NewslettersFeature))]
[Area(AbpCmsKitProAdminRemoteServiceConsts.ModuleName)]
[Route("api/cms-kit-admin/newsletter")]
[RemoteService(true, Name = CmsKitAdminRemoteServiceConsts.RemoteServiceName)]
public class NewsletterRecordAdminController : CmsKitProAdminController, INewsletterRecordAdminAppService
{
  protected INewsletterRecordAdminAppService NewsletterRecordAdminAppService { get; }

  public NewsletterRecordAdminController(INewsletterRecordAdminAppService input)
  {
    NewsletterRecordAdminAppService = input;
  }

  [HttpGet]
  public virtual Task<PagedResultDto<NewsletterRecordDto>> GetListAsync(GetNewsletterRecordsRequestInput input)
  {
    return NewsletterRecordAdminAppService.GetListAsync(input);
  }

  [Route("{id}")]
  [HttpGet]
  public virtual Task<NewsletterRecordWithDetailsDto> GetAsync(Guid id)
  {
    return NewsletterRecordAdminAppService.GetAsync(id);
  }

  [Route("csv-detail")]
  [HttpGet]
  public virtual Task<List<NewsletterRecordCsvDto>> GetNewsletterRecordsCsvDetailAsync(
    GetNewsletterRecordsCsvRequestInput input)
  {
    return NewsletterRecordAdminAppService.GetNewsletterRecordsCsvDetailAsync(input);
  }

  [Route("preferences")]
  [HttpGet]
  public virtual Task<List<string>> GetNewsletterPreferencesAsync()
  {
    return NewsletterRecordAdminAppService.GetNewsletterPreferencesAsync();
  }

  [Route("export-csv")]
  [HttpGet]
  public virtual async Task<IRemoteStreamContent> GetCsvResponsesAsync(
    GetNewsletterRecordsCsvRequestInput input)
  {
    return await NewsletterRecordAdminAppService.GetCsvResponsesAsync(input);
  }
}
