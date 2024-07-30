// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using X.Abp.Payment.Gateways;
using X.Abp.Payment.Requests;

namespace X.Abp.Payment.Web.Pages.Payment;

public class GatewaySelectionModel : PaymentPageModel
{
    protected IPaymentRequestAppService PaymentRequestAppService { get; }

    protected IOptions<PaymentWebOptions> PaymentWebOptions { get; }

    protected IGatewayAppService GatewayAppService { get; }

    [BindProperty]
    public Guid PaymentRequestId { get; set; }

    public List<PaymentGatewayWebConfiguration> Gateways { get; set; }

    public string CheckoutButtonStyle { get; set; }

    public GatewaySelectionModel(
      IPaymentRequestAppService paymentRequestAppService,
      IOptions<PaymentWebOptions> paymentWebOptions,
      IGatewayAppService gatewayAppService)
    {
        PaymentRequestAppService = paymentRequestAppService;
        PaymentWebOptions = paymentWebOptions;
        GatewayAppService = gatewayAppService;
    }

    public virtual ActionResult OnGet()
    {
        return BadRequest();
    }

    public virtual async Task<IActionResult> OnPostAsync()
    {
        CheckoutButtonStyle = PaymentWebOptions.Value.GatewaySelectionCheckoutButtonStyle;
        var gatewaysDtos = (await PaymentRequestAppService.GetAsync(PaymentRequestId)).Products.Any(a => a.PaymentType == PaymentType.Subscription)
            ? await GatewayAppService.GetSubscriptionSupportedGatewaysAsync()
            : await GatewayAppService.GetGatewayConfigurationAsync();
        Gateways = PaymentWebOptions.Value.Gateways.Where(x => gatewaysDtos.Any(a => a.Name == x.Key)).Select(x => x.Value).ToList();
        if (!Gateways.Any())
        {
            throw new ApplicationException("No payment gateway configured!");
        }

        if (Gateways.Count != 1)
        {
            return Page();
        }

        var webConfiguration = Gateways.First();
        return LocalRedirectPreserveMethod($"{webConfiguration.PrePaymentUrl}?paymentRequestId={PaymentRequestId}");
    }
}
