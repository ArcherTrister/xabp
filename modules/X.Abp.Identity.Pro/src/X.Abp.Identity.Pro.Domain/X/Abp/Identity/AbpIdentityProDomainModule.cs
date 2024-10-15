// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

using Volo.Abp;
using Volo.Abp.AspNetCore;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Caching;
using Volo.Abp.Gdpr;
using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Ldap;
using Volo.Abp.Ldap.Localization;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

using X.Abp.Identity.ExternalLoginProviders.Ldap;
using X.Abp.Identity.ExternalLoginProviders.OAuth;
using X.Abp.Identity.Session;

namespace X.Abp.Identity;

[DependsOn(
    typeof(AbpIdentityDomainModule),
    typeof(AbpIdentityProDomainSharedModule),
    typeof(AbpLdapModule),
    typeof(AbpCachingModule),
    typeof(AbpGdprAbstractionsModule),
    typeof(AbpAspNetCoreAbstractionsModule))]
public class AbpIdentityProDomainModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        PreConfigure<IdentityBuilder>(builder =>
        {
            builder.AddUserValidator<MaxUserCountValidator>();
            builder.AddUserValidator<PhoneNumberUserValidator>();
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddScoped(typeof(IUserStore<IdentityUser>), provider => provider.GetService(typeof(IdentityProUserStore)));

        Configure<AbpIdentityOptions>(options =>
        {
            options.ExternalLoginProviders.Add<LdapExternalLoginProvider>(LdapExternalLoginProvider.Name);
            options.ExternalLoginProviders.Add<OAuthExternalLoginProvider>(OAuthExternalLoginProvider.Name);
        });

        context.Services.AddHttpClient(OAuthExternalLoginManager.HttpClientName);
        Configure<AbpLocalizationOptions>(options => options.Resources.Get<IdentityResource>().AddBaseTypes(typeof(LdapResource)));

        context.Services.Replace(ServiceDescriptor.Transient<IIdentityDataSeeder, X.Abp.Identity.IdentityDataSeeder>());
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        if (context.ServiceProvider.GetRequiredService<IOptions<IdentitySessionCleanupOptions>>().Value.IsCleanupEnabled)
        {
            await context.ServiceProvider.GetRequiredService<IBackgroundWorkerManager>().AddAsync(context.ServiceProvider.GetRequiredService<IdentitySessionCleanupBackgroundWorker>());
        }
    }
}
