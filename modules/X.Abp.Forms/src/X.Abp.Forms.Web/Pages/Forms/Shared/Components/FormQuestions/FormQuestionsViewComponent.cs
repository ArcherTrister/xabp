// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Select2;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.VeeValidate;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.Vue;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.Abp.Domain.Entities;

using X.Abp.Forms.Forms;

namespace X.Abp.Forms.Web.Pages.Forms.Shared.Components.FormQuestions;

[Widget(
    StyleFiles = new[] { "/Pages/Forms/Shared/Components/FormQuestions/Default.css" },
    StyleTypes = new[] { typeof(Select2StyleContributor) },
    ScriptTypes = new[] { typeof(VueScriptContributor), typeof(VeeValidateScriptContributor), typeof(FormQuestionsWidgetScriptContributor) },
    AutoInitialize = true)]
[ViewComponent(Name = "FormQuestions")]
public class FormQuestionsViewComponent : AbpViewComponent
{
    protected IFormAppService FormAppService { get; }

    public FormQuestionsViewComponent(IFormAppService formAppService)
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

        var vm = new FormQuestionsViewModel()
        {
            Id = form.Id,
            IsAcceptingResponses = form.IsAcceptingResponses
        };
        return View("~/Pages/Forms/Shared/Components/FormQuestions/Default.cshtml", vm);
    }

    public class FormQuestionsViewModel
    {
        public Guid Id { get; set; }

        public bool IsAcceptingResponses { get; set; }
    }
}
