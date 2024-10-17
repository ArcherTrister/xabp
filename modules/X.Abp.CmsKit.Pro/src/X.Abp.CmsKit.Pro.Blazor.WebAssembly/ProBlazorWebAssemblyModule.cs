// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Components.WebAssembly.Theming;
using Volo.Abp.Modularity;

namespace X.Abp.CmsKit.Pro.Blazor.WebAssembly;

[DependsOn(
    typeof(ProBlazorModule),
    typeof(AbpCmsKitProHttpApiClientModule),
    typeof(AbpAspNetCoreComponentsWebAssemblyThemingModule))]
public class ProBlazorWebAssemblyModule : AbpModule
{
}
