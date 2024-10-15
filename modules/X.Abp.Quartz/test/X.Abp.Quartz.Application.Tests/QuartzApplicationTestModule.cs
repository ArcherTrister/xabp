using Volo.Abp.Modularity;

namespace X.Abp.Quartz;

[DependsOn(
    typeof(AbpQuartzApplicationModule),
    typeof(QuartzDomainTestModule)
    )]
public class QuartzApplicationTestModule : AbpModule
{

}
