// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace X.Abp.Saas.Web.Pages.Saas;

public abstract class SaasPageModel : AbpPageModel
{
    protected SaasPageModel()
    {
        ObjectMapperContext = typeof(AbpSaasWebModule);
    }
}
