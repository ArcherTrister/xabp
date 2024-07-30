// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Volo.Abp.SettingManagement.Web.Pages.SettingManagement;

using X.Abp.Account.Admin.Web.Pages.Account.Components.AccountSettingGroup;
using X.Abp.Account.Localization;

namespace X.Abp.Account.Admin.Web.Settings;

public class AccountSettingManagementPageContributor : ISettingPageContributor
{
    public virtual async Task ConfigureAsync(SettingPageCreationContext context)
    {
        if (!await CheckPermissionsInternalAsync(context))
        {
            return;
        }

        var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AccountResource>>();
        context.Groups.Add(
            new SettingPageGroup(
                "X.Abp.Account",
                l["Menu:Account"],
                typeof(AccountSettingGroupViewComponent)));
    }

    public virtual async Task<bool> CheckPermissionsAsync(SettingPageCreationContext context)
    {
        return await CheckPermissionsInternalAsync(context);
    }

    protected virtual async Task<bool> CheckPermissionsInternalAsync(SettingPageCreationContext context)
    {
        var authorizationService = context.ServiceProvider.GetRequiredService<IAuthorizationService>();

        return await authorizationService.IsGrantedAsync(AccountPermissions.SettingManagement);
    }
}
