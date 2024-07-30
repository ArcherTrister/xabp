// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.MalihuCustomScrollbar;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared.Bundling;
using Volo.Abp.Modularity;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Bundling
{
    [DependsOn(
        typeof(MalihuCustomScrollbarPluginScriptBundleContributor),
        typeof(SharedThemeGlobalScriptContributor))]
    public class LeptonGlobalScriptContributor : BundleContributor
    {
        public override void ConfigureBundle(BundleConfigurationContext context)
        {
            var options = context
                .ServiceProvider
                .GetRequiredService<IOptions<LeptonThemeOptions>>()
                .Value;

            context.Files.AddIfNotContains("/Themes/Lepton/Global/scripts/app.js");

            if (options.EnableDemoFeatures)
            {
                context.Files.AddIfNotContains("/Themes/Lepton/Global/scripts/demo.js");
            }
        }
    }
}
