// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.MainFooter
{
    public abstract class MainFooterComponentBase : ComponentBase
    {
        [Inject]
        protected IOptions<LeptonThemeOptions> Options { get; set; }

        protected RenderFragment FooterFragment { get; set; }

        protected MainFooterComponentBase()
        {
            FooterFragment = builder =>
            {
                builder.OpenComponent(0, Options.Value.FooterComponent);
                builder.CloseComponent();
            };
        }
    }
}
