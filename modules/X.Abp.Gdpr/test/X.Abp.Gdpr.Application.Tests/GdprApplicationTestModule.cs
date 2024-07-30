using Volo.Abp.Modularity;

namespace X.Abp.Gdpr;

[DependsOn(
    typeof(GdprApplicationModule),
    typeof(GdprDomainTestModule)
    )]
public class GdprApplicationTestModule : AbpModule
{

}
