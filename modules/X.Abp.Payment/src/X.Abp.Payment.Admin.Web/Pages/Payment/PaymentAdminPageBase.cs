// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

using Volo.Abp.AspNetCore.Mvc.UI.Layout;

using X.Abp.Payment.Localization;

namespace X.Abp.Payment.Admin.Web.Pages.Payment;

public class PaymentAdminPageBase : Page
{
    [RazorInject]
    public IStringLocalizer<PaymentResource> L { get; set; }

    [RazorInject]
    public IPageLayout PageLayout { get; set; }

    public override Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}
