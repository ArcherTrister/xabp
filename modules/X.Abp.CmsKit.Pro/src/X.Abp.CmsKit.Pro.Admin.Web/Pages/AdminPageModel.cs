using X.Abp.CmsKit.Pro.Admin.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class AdminPageModel : AbpPageModel
{
    protected AdminPageModel()
    {
        LocalizationResourceType = typeof(AdminResource);
        ObjectMapperContext = typeof(AdminWebModule);
    }
}
