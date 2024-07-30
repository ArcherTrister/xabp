using Volo.Abp.Modularity;

namespace X.Abp.CmsKit.Pro;

[DependsOn(
    typeof(ProApplicationModule),
    typeof(ProDomainTestModule)
    )]
public class ProApplicationTestModule : AbpModule
{

}
