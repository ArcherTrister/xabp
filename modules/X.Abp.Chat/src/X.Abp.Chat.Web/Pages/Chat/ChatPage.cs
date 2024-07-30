// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

using X.Abp.Chat.Localization;

namespace X.Abp.Chat.Web.Pages.Chat;

/* Inherit your PageModel classes from this class.
 */
public abstract class ChatPageModel : AbpPageModel
{
    protected ChatPageModel()
    {
        LocalizationResourceType = typeof(AbpChatResource);
        ObjectMapperContext = typeof(AbpChatWebModule);
    }
}
