// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpLanguageManagementHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule))]
public class LanguageManagementConsoleApiClientModule : AbpModule
{
}
