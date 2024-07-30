// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.IdentityServer.Localization;

namespace X.Abp.IdentityServer.Web.Pages.IdentityServer
{
    public abstract class IdentityServerPageModel : AbpPageModel
    {
        protected IdentityServerPageModel()
        {
            LocalizationResourceType = typeof(AbpIdentityServerResource);
            ObjectMapperContext = typeof(AbpIdentityServerProWebModule);
        }
    }
}
