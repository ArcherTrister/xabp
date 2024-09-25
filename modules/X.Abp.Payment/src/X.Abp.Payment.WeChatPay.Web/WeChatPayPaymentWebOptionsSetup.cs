// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Microsoft.Extensions.Options;

using X.Abp.Payment.Web;

namespace X.Abp.Payment.WeChatPay.Web;

public class WeChatPayPaymentWebOptionsSetup : IConfigureOptions<PaymentWebOptions>
{
    protected WeChatPayWebOptions WeChatPayWebOptions { get; }

    public WeChatPayPaymentWebOptionsSetup(IOptions<WeChatPayWebOptions> weChatPayWebOptions)
    {
        WeChatPayWebOptions = weChatPayWebOptions.Value;
    }

    public void Configure(PaymentWebOptions options)
    {
        options.Gateways.Add(new PaymentGatewayWebConfiguration(WeChatPayConsts.GatewayName, WeChatPayConsts.PrePaymentUrl, options.RootUrl.RemovePostFix("/") + WeChatPayConsts.PostPaymentUrl, WeChatPayWebOptions.Recommended, WeChatPayWebOptions.ExtraInfos));
    }
}
