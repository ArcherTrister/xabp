// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.FlagIconCss;
using Volo.Abp.AspNetCore.Mvc.UI.Packages.MalihuCustomScrollbar;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Settings;

using X.Abp.LeptonTheme.Management;
using X.Abp.LeptonTheme.Management.Settings;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton.Bundling
{
    [DependsOn(
        typeof(MalihuCustomScrollbarPluginStyleBundleContributor),
        typeof(FlagIconCssStyleContributor))]
    public class LeptonGlobalStyleContributor : BundleContributor
    {
        public override async Task ConfigureBundleAsync(BundleConfigurationContext context)
        {
            var options = context.ServiceProvider.GetRequiredService<IOptions<LeptonThemeOptions>>().Value;

            if (!options.StylePath.IsNullOrEmpty())
            {
                context.Files.Add(options.StylePath);
            }
            else
            {
                var cssFile = await GetStyleCssFileNameAsync(context, options);
                context.Files.Add($"/Themes/Lepton/Global/Styles/{cssFile}");

                context.Files.RemoveAll(x => x.FileName == (CultureHelper.IsRtl
                    ? "/libs/bootstrap/css/bootstrap.rtl.css"
                    : "/libs/bootstrap/css/bootstrap.css"));
            }
        }

        private static async Task<string> GetStyleCssFileNameAsync(
            BundleConfigurationContext context,
            LeptonThemeOptions options)
        {
            var styleSettingName = options.IsPublicWebsite
                ? LeptonThemeSettingNames.PublicLayoutStyle
                : LeptonThemeSettingNames.Style;

            var style = await context.ServiceProvider
                .GetRequiredService<ISettingProvider>()
                .GetOrNullAsync(styleSettingName);

            var rtlStringExtension = CultureHelper.IsRtl ? ".rtl" : string.Empty;

            if (string.Equals(style, LeptonStyle.Style1.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return $"lepton1{rtlStringExtension}.css";
            }
            else if (string.Equals(style, LeptonStyle.Style2.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return $"lepton2{rtlStringExtension}.css";
            }
            else if (string.Equals(style, LeptonStyle.Style3.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return $"lepton3{rtlStringExtension}.css";
            }
            else if (string.Equals(style, LeptonStyle.Style4.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return $"lepton4{rtlStringExtension}.css";
            }
            else if (string.Equals(style, LeptonStyle.Style5.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return $"lepton5{rtlStringExtension}.css";
            }
            else if (string.Equals(style, LeptonStyle.Style6.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                return $"lepton6{rtlStringExtension}.css";
            }

            return $"lepton1{rtlStringExtension}.css";
        }
    }
}
