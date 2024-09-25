// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Essensoft.Paylink.WeChatPay;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Json;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;

namespace X.Abp.Payment.WeChatPay
{
    [DependsOn(typeof(AbpJsonAbstractionsModule), typeof(AbpPaymentDomainModule), typeof(AbpPaymentWeChatPayDomainSharedModule))]
    public class AbpPaymentWeChatPayDomainModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<PaymentOptions>(options => options.Gateways.Add(
                new PaymentGatewayConfiguration(
                    WeChatPayConsts.GatewayName,
                    new FixedLocalizableString(WeChatPayConsts.GatewayName),
                    false,
                    typeof(WeChatPayPaymentGateway))));

            var section = context.Services.GetConfiguration().GetSection("Payment:WeChatPay");

            Configure<WeChatPayOptions>(section);

            context.Services.AddWeChatPay();

            /*
            context.Services.AddTransient<IWeChatPayHttpClient>(provider =>
            {
                // TODO: WeChatPayOptions
                var weChatPayOptions = provider.GetService<IOptions<WeChatPayOptions>>().Value;
                // IWeChatPayHttpClient
                return new WeChatPayHttpClient(weChatPayOptions);
            });
            */
        }
    }
}
