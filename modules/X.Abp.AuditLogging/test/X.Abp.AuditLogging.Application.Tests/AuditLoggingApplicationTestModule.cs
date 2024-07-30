using Volo.Abp.Modularity;

namespace X.Abp.AuditLogging;

[DependsOn(
    typeof(AuditLoggingApplicationModule),
    typeof(AuditLoggingDomainTestModule)
    )]
public class AuditLoggingApplicationTestModule : AbpModule
{

}
