// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using Volo.Abp.Identity;
using Volo.Abp.Settings;
using Volo.Abp.Users;

using X.Abp.Account.Localization;
using X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.Password;
using X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.PersonalInfo;
using X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.ProfilePicture;
using X.Abp.Account.Public.Web.Pages.Account.Components.ProfileManagementGroup.TwoFactor;
using X.Abp.Identity;
using X.Abp.Identity.Settings;

namespace X.Abp.Account.Public.Web.ProfileManagement;

public class AccountProfileManagementPageContributor : IProfileManagementPageContributor
{
  public virtual async Task ConfigureAsync(ProfileManagementPageCreationContext context)
  {
    var l = context.ServiceProvider.GetRequiredService<IStringLocalizer<AccountResource>>();

    context.Groups.Add(
        new ProfileManagementPageGroup(
            "Volo-Abp-Account-Picture",
            l["ProfileTab:Picture"],
            typeof(AccountProfilePictureManagementGroupViewComponent)));

    if (await IsPasswordChangeEnabled(context))
    {
      context.Groups.Add(
          new ProfileManagementPageGroup(
              "Volo-Abp-Account-Password",
              l["ProfileTab:Password"],
              typeof(AccountProfilePasswordManagementGroupViewComponent)));
    }

    context.Groups.Add(
        new ProfileManagementPageGroup(
            "Volo-Abp-Account-PersonalInfo",
            l["ProfileTab:PersonalInfo"],
            typeof(AccountProfilePersonalInfoManagementGroupViewComponent)));

    var identityProTwoFactorManager = context.ServiceProvider.GetRequiredService<IdentityProTwoFactorManager>();
    var settingProvider = context.ServiceProvider.GetRequiredService<ISettingProvider>();
    if (await identityProTwoFactorManager.IsOptionalAsync() &&
        await settingProvider.GetAsync<bool>(IdentityProSettingNames.TwoFactor.UsersCanChange))
    {
      context.Groups.Add(
          new ProfileManagementPageGroup(
              "Volo-Abp-Account-TwoFactor",
              l["ProfileTab:TwoFactor"],
              typeof(AccountProfileTwoFactorManagementGroupViewComponent)));
    }
  }

  protected virtual async Task<bool> IsPasswordChangeEnabled(ProfileManagementPageCreationContext context)
  {
    var userManager = context.ServiceProvider.GetRequiredService<IdentityUserManager>();
    var currentUser = context.ServiceProvider.GetRequiredService<ICurrentUser>();

    var user = await userManager.GetByIdAsync(currentUser.GetId());

    return !user.IsExternal;
  }
}
