// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Features;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Ldap;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Settings;
using Volo.Abp.VirtualFileSystem;

namespace X.Abp.Identity;

[DependsOn(
    typeof(AbpIdentityDomainSharedModule),
    typeof(AbpVirtualFileSystemModule),
    typeof(AbpLocalizationModule),
    typeof(AbpFeaturesModule),
    typeof(AbpSettingsModule),
    typeof(AbpLdapAbstractionsModule))]
public class AbpIdentityProDomainSharedModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpIdentityProDomainSharedModule>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Get<IdentityResource>()
                .AddVirtualJson("/X/Abp/Identity/Localization/Resources");
        });

        Configure<AbpExceptionLocalizationOptions>(options => options.MapCodeNamespace("Volo.Abp.Identity", typeof(IdentityResource)));
    }
}
