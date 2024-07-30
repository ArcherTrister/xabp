// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Volo.Abp;
using Volo.Abp.GlobalFeatures;
using Volo.CmsKit.Admin;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Admin.Contact;

[Authorize(AbpCmsKitProAdminPermissions.Contact.SettingManagement)]
[RequiresGlobalFeature(typeof(ContactFeature))]
[RemoteService(true, Name = CmsKitAdminRemoteServiceConsts.RemoteServiceName)]
[Area(AbpCmsKitProAdminRemoteServiceConsts.ModuleName)]
[Route("api/cms-kit-admin/contact/settings")]
public class ContactSettingController :
  CmsKitProAdminController,
  IContactSettingsAppService
{
    protected IContactSettingsAppService ContactSettingsAppService { get; }

    public ContactSettingController(IContactSettingsAppService contactSettingsAppService)
    {
        ContactSettingsAppService = contactSettingsAppService;
    }

    [HttpGet]
    public virtual Task<CmsKitContactSettingDto> GetAsync()
    {
        return ContactSettingsAppService.GetAsync();
    }

    [HttpPost]
    public virtual Task UpdateAsync(UpdateCmsKitContactSettingDto input)
    {
        return ContactSettingsAppService.UpdateAsync(input);
    }
}
