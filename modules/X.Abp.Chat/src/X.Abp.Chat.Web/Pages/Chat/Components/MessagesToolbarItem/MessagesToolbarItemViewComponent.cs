// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;

namespace X.Abp.Chat.Web.Pages.Chat.Components.MessagesToolbarItem;

public class MessagesToolbarItemViewComponent : AbpViewComponent
{
    public virtual IViewComponentResult Invoke()
    {
        return View("/Pages/Chat/Components/MessagesToolbarItem/Default.cshtml");
    }
}
