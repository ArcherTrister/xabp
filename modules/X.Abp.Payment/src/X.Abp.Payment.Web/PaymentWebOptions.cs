// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Payment.Web;

public class PaymentWebOptions
{
    public string CallbackUrl { get; set; }

    public string RootUrl { get; set; }

    public string GatewaySelectionCheckoutButtonStyle { get; set; }

    public PaymentGatewayWebConfigurationDictionary Gateways { get; }

    public PaymentWebOptions()
    {
        Gateways = new PaymentGatewayWebConfigurationDictionary();
    }
}
