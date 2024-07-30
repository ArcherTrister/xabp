using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.Payment.Blazor.WebAssembly;

[DependsOn(
    typeof(PaymentBlazorModule),
    typeof(PaymentHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule)
    )]
public class PaymentBlazorWebAssemblyModule : AbpModule
{

}
