// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc;

using X.Abp.Account.Localization;

namespace X.Abp.Account.Public.Web.Areas.Account.Controllers;

public abstract class AccountControllerBase : AbpController
{
    protected AccountControllerBase()
    {
        ObjectMapperContext = typeof(AbpAccountPublicWebModule);
        LocalizationResource = typeof(AccountResource);
    }
}
