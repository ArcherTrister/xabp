// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

using X.Abp.TextTemplateManagement.Localization;

namespace X.Abp.TextTemplateManagement.Web.Pages.TextTemplateManagement;

/* Inherit your PageModel classes from this class.
 */
public abstract class TextTemplateManagementPageModel : AbpPageModel
{
    protected TextTemplateManagementPageModel()
    {
        LocalizationResourceType = typeof(TextTemplateManagementResource);
        ObjectMapperContext = typeof(AbpTextTemplateManagementWebModule);
    }
}
