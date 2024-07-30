using Volo.Abp.Autofac;
using Volo.Abp.Http.Client.IdentityModel;
using Volo.Abp.Modularity;

namespace X.Abp.AuditLogging;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpAuditLoggingHttpApiClientModule),
    typeof(AbpHttpClientIdentityModelModule)
    )]
public class AuditLoggingConsoleApiClientModule : AbpModule
{

}
