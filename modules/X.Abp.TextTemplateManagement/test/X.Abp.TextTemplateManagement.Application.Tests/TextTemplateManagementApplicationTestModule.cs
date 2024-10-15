using Volo.Abp.Modularity;

namespace X.Abp.TextTemplateManagement;

[DependsOn(
    typeof(AbpTextTemplateManagementApplicationModule),
    typeof(TextTemplateManagementDomainTestModule)
    )]
public class TextTemplateManagementApplicationTestModule : AbpModule
{

}
