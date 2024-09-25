// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Settings;
using Volo.Abp.Settings;

using X.Abp.Identity;

namespace X.Abp.Account.Emailing;

public class AccountIdentityUserCreatedEventHandler :
    IDistributedEventHandler<IdentityUserCreatedEto>,
    ITransientDependency
{
  protected IdentityUserManager UserManager { get; }

  protected IAccountEmailer AccountEmailer { get; }

  protected ISettingProvider SettingProvider { get; }

  public AccountIdentityUserCreatedEventHandler(
      IdentityUserManager userManager,
      IAccountEmailer accountEmailer,
      ISettingProvider settingProvider)
  {
    UserManager = userManager;
    AccountEmailer = accountEmailer;
    SettingProvider = settingProvider;
  }

  public virtual async Task HandleEventAsync(IdentityUserCreatedEto eventData)
  {
    if (eventData.Properties["SendConfirmationEmail"].Equals(true.ToString(), System.StringComparison.OrdinalIgnoreCase) &&
        await SettingProvider.IsTrueAsync(IdentitySettingNames.SignIn.RequireConfirmedEmail))
    {
      var user = await UserManager.GetByIdAsync(eventData.Id);
      var confirmationToken = await UserManager.GenerateEmailConfirmationTokenAsync(user);
      await AccountEmailer.SendEmailConfirmationLinkAsync(
          user,
          confirmationToken,
          eventData.Properties.GetOrDefault("AppName") ?? "MVC");
    }
  }
}
