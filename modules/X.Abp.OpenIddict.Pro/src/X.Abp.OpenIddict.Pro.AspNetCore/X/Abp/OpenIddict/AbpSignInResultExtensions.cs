// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Identity;

using Volo.Abp.OpenIddict;

namespace X.Abp.OpenIddict;

public static class AbpSignInResultExtensions
{
    public static string ToIdentitySecurityLogAction(this SignInResult result)
    {
        if (result.Succeeded)
        {
            return OpenIddictSecurityLogActionConsts.LoginSucceeded;
        }

        return result.IsLockedOut
            ? OpenIddictSecurityLogActionConsts.LoginLockedout
            : result.RequiresTwoFactor
            ? OpenIddictSecurityLogActionConsts.LoginRequiresTwoFactor
            : result.IsNotAllowed
            ? OpenIddictSecurityLogActionConsts.LoginNotAllowed
            : !result.Succeeded ? OpenIddictSecurityLogActionConsts.LoginFailed : OpenIddictSecurityLogActionConsts.LoginFailed;
    }
}
