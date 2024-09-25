// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.CmsKit.Localization;

namespace X.Abp.CmsKit.Pro.Admin.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class AdminPageModel : AbpPageModel
{
    protected AdminPageModel()
    {
        LocalizationResourceType = typeof(CmsKitResource);
        ObjectMapperContext = typeof(CmsKitResource);
    }
}
