// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace X.Abp.Forms.Web.Pages.Forms.Shared.Components.ViewForm;

public class ViewFormWidgetScriptContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add("/client-proxies/form-proxy.js");
        context.Files.Add("/Pages/Forms/Shared/Components/ViewForm/Vue-email-property.js");
        context.Files.Add("/Pages/Forms/Shared/Components/ViewForm/Vue-answer.js");
        context.Files.Add("/Pages/Forms/Shared/Components/ViewForm/Default.js");
    }
}
