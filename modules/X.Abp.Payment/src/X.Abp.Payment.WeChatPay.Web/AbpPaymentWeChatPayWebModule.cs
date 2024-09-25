// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Payment.Web;

namespace X.Abp.Payment.WeChatPay.Web
{
    [DependsOn(typeof(AbpPaymentWebModule), typeof(AbpPaymentWeChatPayDomainSharedModule))]
    public class AbpPaymentWeChatPayWebModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.TryAddEnumerable(ServiceDescriptor.Transient<IConfigureOptions<PaymentWebOptions>, WeChatPayPaymentWebOptionsSetup>());

            Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpPaymentWeChatPayWebModule>());

            Configure<WeChatPayWebOptions>(context.Services.GetConfiguration().GetSection("Payment:WeChatPayWeb"));
        }
    }
}
