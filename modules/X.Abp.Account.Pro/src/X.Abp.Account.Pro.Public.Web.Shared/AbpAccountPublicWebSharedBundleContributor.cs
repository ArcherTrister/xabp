// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

using X.Abp.Account.AuthorityDelegation;

namespace X.Abp.Account.Public.Web;

public class AbpAccountPublicWebSharedBundleContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        if (context.ServiceProvider.GetRequiredService<IOptions<AbpAccountAuthorityDelegationOptions>>().Value.EnableDelegatedImpersonation)
        {
            context.Files.Add("/Pages/Account/AuthorityDelegation/account-authority-delegation-global.js");
            context.Files.Add("/Pages/Account/LinkUsers/account-link-user-global.js");
        }
    }
}
