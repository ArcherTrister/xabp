// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AutoMapper;
using Volo.Abp.Http.ProxyScripting.Generators.JQuery;
using Volo.Abp.Modularity;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Payment.Localization;

namespace X.Abp.Payment.Web;

[DependsOn(typeof(AbpAspNetCoreMvcModule), typeof(AbpPaymentApplicationContractsModule), typeof(AbpAutoMapperModule))]
public class AbpPaymentWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options => options.AddAssemblyResource(typeof(PaymentResource), typeof(AbpPaymentWebModule).Assembly, typeof(AbpPaymentApplicationContractsModule).Assembly));
        PreConfigure<IMvcBuilder>(mvcBuilder => mvcBuilder.AddApplicationPartIfNotExists(typeof(AbpPaymentWebModule).Assembly));
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options => options.FileSets.AddEmbedded<AbpPaymentWebModule>("X.Abp.Payment.Web"));
        context.Services.AddAutoMapperObjectMapper<AbpPaymentWebModule>();
        Configure<AbpAutoMapperOptions>(options => options.AddMaps<AbpPaymentWebModule>(true));
        Configure<RazorPagesOptions>(options => { });
        Configure<DynamicJavaScriptProxyOptions>(options => options.DisableModule(AbpPaymentCommonRemoteServiceConsts.ModuleName));
    }
}
