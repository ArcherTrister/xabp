// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using Volo.Abp.DependencyInjection;

using Volo.Abp.Identity;

using IdentityUser = Volo.Abp.Identity.IdentityUser;

namespace X.Abp.Account;
public class IdentityUserTwoFactorChecker : ITransientDependency
{
    protected IdentityUserManager UserManager { get; }

    public IdentityUserTwoFactorChecker(IdentityUserManager userManager)
    {
        UserManager = userManager;
    }

    public virtual async Task<bool> CanEnabledAsync(IdentityUser user)
    {
        // var providers = await UserManager.GetValidTwoFactorProvidersAsync(user);
        // return providers.Count != 0 && (providers.Count != 1 || !providers.Contains("Authenticator"));
        IList<string> stringList = await UserManager.GetValidTwoFactorProvidersAsync(user);
        return stringList.Count != 0 && (stringList.Count != 1 || !stringList.Contains("Authenticator") || user.HasAuthenticator());
    }

    public virtual async Task CheckAsync(IdentityUser user)
    {
        if (await CanEnabledAsync(user))
        {
            return;
        }

        (await UserManager.SetTwoFactorEnabledAsync(user, false)).CheckIdentityErrors();
    }
}
