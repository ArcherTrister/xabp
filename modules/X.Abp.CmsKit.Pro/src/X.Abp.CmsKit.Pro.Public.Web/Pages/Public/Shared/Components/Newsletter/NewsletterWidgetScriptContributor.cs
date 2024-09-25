// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc.UI.Bundling;

namespace X.Abp.CmsKit.Pro.Public.Web.Pages.Public.Shared.Components.Newsletter;

public class NewsletterWidgetScriptContributor : BundleContributor
{
    public override void ConfigureBundle(BundleConfigurationContext context)
    {
        context.Files.Add("/client-proxies/cms-kit-pro-common-proxy.js");
        context.Files.Add("/Pages/Public/Shared/Components/Newsletter/Default.js");
    }
}
