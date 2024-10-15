using Volo.Abp.Modularity;

namespace X.Abp.AuditLogging;

[DependsOn(
    typeof(AbpAuditLoggingApplicationModule),
    typeof(AuditLoggingTestBaseModule)
    )]
public class AuditLoggingApplicationTestModule : AbpModule
{

}
