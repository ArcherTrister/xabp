// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace X.Abp.Forms.Web.Pages.Forms.Shared.Components.FormQuestions;

public class FormQuestionsWidgetScriptContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add("/client-proxies/form-proxy.js");
        context.Files.Add("/Pages/Forms/Shared/Components/FormQuestions/Vue-question-choice.js");
        context.Files.Add("/Pages/Forms/Shared/Components/FormQuestions/Vue-question-types.js");
        context.Files.Add("/Pages/Forms/Shared/Components/FormQuestions/Vue-question-item.js");
        context.Files.Add("/Pages/Forms/Shared/Components/FormQuestions/Default.js");
    }
}
