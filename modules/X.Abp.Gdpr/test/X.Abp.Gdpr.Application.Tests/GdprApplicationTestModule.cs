using Volo.Abp.Modularity;

namespace X.Abp.Gdpr;

[DependsOn(
    typeof(AbpGdprApplicationModule),
    typeof(GdprDomainTestModule)
    )]
public class GdprApplicationTestModule : AbpModule
{

}
