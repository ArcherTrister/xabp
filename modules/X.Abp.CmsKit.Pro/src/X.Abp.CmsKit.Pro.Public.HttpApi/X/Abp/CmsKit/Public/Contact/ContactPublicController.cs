// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Public.Contact;

[Route("api/cms-kit-public/contacts")]
[RequiresGlobalFeature(typeof(ContactFeature))]
[Area(AbpCmsKitProPublicRemoteServiceConsts.ModuleName)]
[RemoteService(true, Name = AbpCmsKitProPublicRemoteServiceConsts.RemoteServiceName)]
public class ContactPublicController : CmsKitProPublicController, IContactPublicAppService
{
    protected IContactPublicAppService ContactPublicAppService { get; }

    public ContactPublicController(
      IContactPublicAppService contactPublicAppService)
    {
        ContactPublicAppService = contactPublicAppService;
    }

    [HttpPost]
    public virtual async Task SendMessageAsync(ContactCreateInput input)
    {
        await ContactPublicAppService.SendMessageAsync(input);
    }
}
