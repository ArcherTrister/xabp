// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.Configuration;
using Volo.Abp.AspNetCore.Mvc.UI.Theming;
using Volo.Abp.DependencyInjection;

namespace X.Abp.AspNetCore.Mvc.UI.Theme.Lepton
{
    [ThemeName(Name)]
    public class LeptonTheme : ITheme, ITransientDependency
    {
        public const string Name = "Lepton";

        protected IConfiguration Configuration { get; }

        public LeptonTheme(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public virtual string GetLayout(string name, bool fallbackToDefault = true)
        {
            switch (name)
            {
                case StandardLayouts.Application:
                    return GetLayoutFromConfig("Application") ?? "~/Themes/Lepton/Layouts/Application/Default.cshtml";
                case StandardLayouts.Account:
                    return GetLayoutFromConfig("Account") ?? "~/Themes/Lepton/Layouts/Account/Default.cshtml";
                case StandardLayouts.Public:
                    return GetLayoutFromConfig("Public") ?? "~/Themes/Lepton/Layouts/Public/Default.cshtml";
                case StandardLayouts.Empty:
                    return GetLayoutFromConfig("Empty") ?? "~/Themes/Lepton/Layouts/Empty/Default.cshtml";
                default:
                    return fallbackToDefault ? "~/Themes/Lepton/Layouts/Application/Default.cshtml" : null;
            }
        }

        private string GetLayoutFromConfig(string layoutName)
        {
            return Configuration["LeptonTheme:Layouts:" + layoutName];
        }
    }
}
