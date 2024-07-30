// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

using X.Abp.CmsKit.Public.Contact;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Contact;

[ViewComponent(Name = "CmsContact")]
[Widget(ScriptTypes = new Type[] { typeof(ContactWidgetScriptContributor) }, RefreshUrl = "/CmsKitProPublicWidgets/Contact", AutoInitialize = true)]
public class ContactViewComponent : AbpViewComponent
{
    protected IContactPublicAppService ContactPublicAppService { get; }

    public ContactViewComponent(IContactPublicAppService contactPublicAppService)
    {
        ContactPublicAppService = contactPublicAppService;
    }

    public virtual IViewComponentResult Invoke(string contactName)
    {
        var model = new ContactViewModel
        {
            ContactName = contactName
        };
        return View("~/Pages/Public/Shared/Components/Contact/Default.cshtml", model);
    }
}
