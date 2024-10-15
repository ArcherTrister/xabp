// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.OpenIddict.Localization;

namespace X.Abp.OpenIddict.Web.Pages.OpenIddict;

public abstract class OpenIddictProPageModel : AbpPageModel
{
    protected OpenIddictProPageModel()
    {
        LocalizationResourceType = typeof(AbpOpenIddictResource);
        ObjectMapperContext = typeof(AbpOpenIddictProWebModule);
    }
}
