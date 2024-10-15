using Volo.Abp.Modularity;

namespace X.Abp.CmsKit.Pro;

[DependsOn(
    typeof(CmsKitProApplicationModule),
    typeof(CmsKitProDomainTestModule)
    )]
public class CmsKitProApplicationTestModule : AbpModule
{

}
