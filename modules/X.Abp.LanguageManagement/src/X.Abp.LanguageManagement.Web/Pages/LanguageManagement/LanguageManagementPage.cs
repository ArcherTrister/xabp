// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

using X.Abp.LanguageManagement.Localization;

namespace X.Abp.LanguageManagement.Web.Pages.LanguageManagement;

/* Inherit your PageModel classes from this class.
 */
public abstract class LanguageManagementPageModel : AbpPageModel
{
    protected LanguageManagementPageModel()
    {
        LocalizationResourceType = typeof(LanguageManagementResource);
        ObjectMapperContext = typeof(AbpLanguageManagementWebModule);
    }
}
