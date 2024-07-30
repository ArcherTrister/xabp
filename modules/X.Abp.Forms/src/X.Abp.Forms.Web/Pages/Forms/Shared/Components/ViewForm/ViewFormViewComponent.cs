// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.VeeValidate;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Vue;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;

using X.Abp.Forms.Responses;

namespace X.Abp.Forms.Web.Pages.Forms.Shared.Components.ViewForm;

[Widget(
    StyleFiles = new[] { "/Pages/Forms/Shared/Components/ViewForm/Default.css" },
    ScriptTypes = new[] { typeof(VueScriptContributor), typeof(VeeValidateScriptContributor), typeof(ViewFormWidgetScriptContributor) },
    AutoInitialize = true)]
[ViewComponent(Name = "ViewForm")]
public class ViewFormViewComponent : AbpViewComponent
{
    protected IResponseAppService ResponseAppService { get; }

    public ViewFormViewComponent(IResponseAppService responseAppService)
    {
        ResponseAppService = responseAppService;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync(Guid formId, bool preview = false)
    {
        var form = await ResponseAppService.GetFormDetailsAsync(formId);

        var vm = new ViewFormViewModel
        {
            Id = form.Id,
            Preview = preview
        };

        return View("~/Pages/Forms/Shared/Components/ViewForm/Default.cshtml", vm);
    }
}
