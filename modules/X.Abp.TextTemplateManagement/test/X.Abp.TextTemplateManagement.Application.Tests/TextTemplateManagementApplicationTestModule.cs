using Volo.Abp.Modularity;

namespace X.Abp.TextTemplateManagement;

[DependsOn(
    typeof(TextTemplateManagementApplicationModule),
    typeof(TextTemplateManagementDomainTestModule)
    )]
public class TextTemplateManagementApplicationTestModule : AbpModule
{

}
