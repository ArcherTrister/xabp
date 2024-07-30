// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace X.Abp.Forms.Web.Pages.Forms.Shared.Components.FormResponses;

public class FormResponsesWidgetScriptContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add("/client-proxies/form-proxy.js");
        context.Files.Add("/Pages/Forms/Shared/Components/FormResponses/Vue-block-response-component.js");
        context.Files.Add("/Pages/Forms/Shared/Components/FormResponses/Vue-response-chart.js");
        context.Files.Add("/Pages/Forms/Shared/Components/FormResponses/Vue-response-answers.js");
        context.Files.Add("/Pages/Forms/Shared/Components/FormResponses/Default.js");
    }
}
