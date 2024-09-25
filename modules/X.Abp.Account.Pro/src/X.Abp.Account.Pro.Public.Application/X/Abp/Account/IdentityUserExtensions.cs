// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Data;
using Volo.Abp.Identity;

namespace X.Abp.Account;
public static class IdentityUserExtensions
{
    public static IdentityUser SetAuthenticator(
      this IdentityUser user,
      bool authenticator)
    {
        user.SetProperty("Authenticator", authenticator);
        return user;
    }

    public static bool HasAuthenticator(this IdentityUser user)
    {
        return user.GetProperty<bool>("Authenticator");
    }
}
