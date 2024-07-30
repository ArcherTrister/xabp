using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;

namespace X.Abp.AspNetCore.Components.Web.LeptonTheme.Components.ApplicationLayout.MainFooter
{
    public abstract class MainFooterComponentBase : ComponentBase
    {
        [Inject] protected IOptions<LeptonThemeOptions> Options { get; set; }

        protected RenderFragment FooterFragment { get; set; }

        public MainFooterComponentBase()
        {
            FooterFragment = builder =>
            {
                builder.OpenComponent(0, Options.Value.FooterComponent);
                builder.CloseComponent();
            };
        }
    }
}
