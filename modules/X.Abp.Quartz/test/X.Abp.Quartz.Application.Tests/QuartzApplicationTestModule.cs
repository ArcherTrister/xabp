using Volo.Abp.Modularity;

namespace X.Abp.Quartz;

[DependsOn(
    typeof(QuartzApplicationModule),
    typeof(QuartzDomainTestModule)
    )]
public class QuartzApplicationTestModule : AbpModule
{

}
