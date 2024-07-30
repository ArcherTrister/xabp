// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;
using Volo.Abp.Features;

using X.Abp.Chat.Features;

namespace X.Abp.Chat.Web.Pages.Chat;

[RequiresFeature(AbpChatFeatures.Enable)]
public class IndexModel : AbpPageModel
{
    public void OnGet()
    {
    }
}
