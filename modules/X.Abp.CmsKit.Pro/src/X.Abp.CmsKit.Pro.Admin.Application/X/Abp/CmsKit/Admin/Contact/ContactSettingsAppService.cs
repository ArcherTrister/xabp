// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

using Volo.Abp.GlobalFeatures;
using Volo.Abp.SettingManagement;

using X.Abp.CmsKit.Admin.Permissions;
using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.CmsKit.Admin.Contact;

[RequiresGlobalFeature(ContactFeature.Name)]
[Authorize(AbpCmsKitProAdminPermissions.Contact.SettingManagement)]
public class ContactSettingsAppService : CmsKitProAdminAppServiceBase, IContactSettingsAppService
{
    protected ISettingManager SettingManager { get; }

    public ContactSettingsAppService(ISettingManager settingManager)
    {
        SettingManager = settingManager;
    }

    public virtual async Task<CmsKitContactSettingDto> GetAsync()
    {
        var str = await SettingManager.GetOrNullForCurrentTenantAsync(CmsKitProSettingNames.Contact.ReceiverEmailAddress, true);
        return new CmsKitContactSettingDto()
        {
            ReceiverEmailAddress = str
        };
    }

    public virtual async Task UpdateAsync(UpdateCmsKitContactSettingDto input)
    {
        await SettingManager.SetForCurrentTenantAsync(CmsKitProSettingNames.Contact.ReceiverEmailAddress, input.ReceiverEmailAddress, false);
    }
}
