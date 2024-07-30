// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Public.Newsletters;

[Area(AbpCmsKitProPublicRemoteServiceConsts.ModuleName)]
[RemoteService(true, Name = AbpCmsKitProPublicRemoteServiceConsts.RemoteServiceName)]
[Route("api/cms-kit-public/newsletter")]
[RequiresGlobalFeature(typeof(NewslettersFeature))]
public class NewsletterRecordPublicController : CmsKitProPublicController, INewsletterRecordPublicAppService
{
    protected INewsletterRecordPublicAppService NewsletterRecordPublicAppService { get; }

    public NewsletterRecordPublicController(INewsletterRecordPublicAppService preference)
    {
        NewsletterRecordPublicAppService = preference;
    }

    [HttpPost]
    public virtual Task CreateAsync(CreateNewsletterRecordInput input)
    {
        return NewsletterRecordPublicAppService.CreateAsync(input);
    }

    [Route("emailAddress")]
    [HttpGet]
    public virtual Task<List<NewsletterPreferenceDetailsDto>> GetNewsletterPreferencesAsync([Required] string emailAddress)
    {
        return NewsletterRecordPublicAppService.GetNewsletterPreferencesAsync(emailAddress);
    }

    [HttpPut]
    public virtual async Task UpdatePreferencesAsync(UpdatePreferenceRequestInput input)
    {
        await NewsletterRecordPublicAppService.UpdatePreferencesAsync(input);
    }

    [Route("preference-options")]
    [HttpGet]
    public virtual async Task<NewsletterEmailOptionsDto> GetOptionByPreferenceAsync(string preference)
    {
        return await NewsletterRecordPublicAppService.GetOptionByPreferenceAsync(preference);
    }
}
