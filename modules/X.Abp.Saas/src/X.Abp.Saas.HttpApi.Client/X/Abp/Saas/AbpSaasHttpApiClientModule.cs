// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.FeatureManagement;
using Volo.Abp.Http.Client;
using Volo.Abp.Modularity;

namespace X.Abp.Saas;

[DependsOn(
    typeof(AbpSaasApplicationContractsModule),
    typeof(AbpHttpClientModule),
    typeof(AbpFeatureManagementHttpApiClientModule))]
public class AbpSaasHttpApiClientModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(typeof(AbpSaasApplicationContractsModule).Assembly, AbpSaasRemoteServiceConsts.RemoteServiceName);
    }
}
