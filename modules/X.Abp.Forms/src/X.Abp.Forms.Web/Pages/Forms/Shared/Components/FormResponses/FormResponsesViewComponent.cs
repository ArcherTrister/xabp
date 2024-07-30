// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.ChartJs;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Select2;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Vue;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Domain.Entities;

using X.Abp.Forms.Forms;

namespace X.Abp.Forms.Web.Pages.Forms.Shared.Components.FormResponses;

[Widget(
    StyleFiles = new[] { "/Pages/Forms/Shared/Components/FormResponses/Default.css" },
    StyleTypes = new[] { typeof(Select2StyleContributor) },
    ScriptTypes = new[] { typeof(VueScriptContributor), typeof(ChartjsScriptContributor), typeof(FormResponsesWidgetScriptContributor) },
    AutoInitialize = true)]
[ViewComponent(Name = "FormResponses")]
public class FormResponsesViewComponent : AbpViewComponent
{
    protected IFormAppService FormAppService { get; }

    public FormResponsesViewComponent(IFormAppService formAppService)
    {
        FormAppService = formAppService;
    }

    public virtual async Task<IViewComponentResult> InvokeAsync(Guid id)
    {
        var form = await FormAppService.GetAsync(id);
        if (form == null)
        {
            throw new EntityNotFoundException();
        }

        var vm = new FormResponsesViewModel
        {
            Id = form.Id,
            IsAcceptingResponses = form.IsAcceptingResponses
        };
        return View("~/Pages/Forms/Shared/Components/FormResponses/Default.cshtml", vm);
    }

    public class FormResponsesViewModel
    {
        public Guid Id { get; set; }

        public bool IsAcceptingResponses { get; set; }
    }
}
