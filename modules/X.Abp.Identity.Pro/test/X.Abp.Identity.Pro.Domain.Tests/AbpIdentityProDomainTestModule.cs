using Microsoft.Extensions.DependencyInjection;

using Volo.Abp;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.Threading;
using Volo.Abp.Uow;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Identity.EntityFrameworkCore;

namespace X.Abp.Identity;

/* Domain tests are configured to use the EF Core provider.
 * You can switch to MongoDB, however your domain tests should be
 * database independent anyway.
 */
[DependsOn(
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpIdentityProEntityFrameworkCoreTestModule),
    typeof(AbpIdentityProTestBaseModule)
    )]
public class AbpIdentityProDomainTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpDistributedEntityEventOptions>(options =>
        {
            options.AutoEventSelectors.Add<IdentityUser>();
        });

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpIdentityProDomainTestModule>();
        });

        //Configure<AbpLocalizationOptions>(options =>
        //{
        //    options.Resources
        //        .Get<IdentityResource>()
        //        .AddVirtualJson("/X/Abp/Identity/LocalizationExtensions");
        //});

        Configure<PermissionManagementOptions>(options =>
        {
            options.IsDynamicPermissionStoreEnabled = false;
            options.SaveStaticPermissionsToDatabase = false;
        });

        Configure<AbpUnitOfWorkDefaultOptions>(options =>
        {
            options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        });

        //Configure<AbpSettingOptions>(options =>
        //{
        //    options.ValueProviders.Add<TestSettingValueProvider>();
        //});
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        SeedTestData(context);
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        using (var scope = context.ServiceProvider.CreateScope())
        {
            AsyncHelper.RunSync(() => scope.ServiceProvider
                .GetRequiredService<TestPermissionDataBuilder>()
                .Build());
        }
    }
}
