// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;

using Volo.Abp;
using Volo.Abp.AutoMapper;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Domain.Entities.Events.Distributed;
using Volo.Abp.Localization;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending.Modularity;
using Volo.Abp.Threading;

using X.Abp.LanguageManagement.External;
using X.Abp.LanguageManagement.Localization;
using X.Abp.ObjectExtending;

namespace X.Abp.LanguageManagement;

[DependsOn(
    typeof(AbpLanguageManagementDomainSharedModule),
    typeof(AbpAutoMapperModule),
    typeof(AbpDddDomainModule),
    typeof(AbpCachingModule))]
#pragma warning disable CA1001 // 具有可释放字段的类型应该是可释放的
public class AbpLanguageManagementDomainModule : AbpModule
#pragma warning restore CA1001 // 具有可释放字段的类型应该是可释放的
{
    private static readonly OneTimeRunner OneTimeRunner = new();
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        OneTimeRunner.Run(() =>
        {
            ModuleExtensionConfigurationHelper.ApplyEntityConfigurationToEntity(
                LanguageManagementModuleExtensionConsts.ModuleName,
                LanguageManagementModuleExtensionConsts.EntityNames.Language,
                typeof(Language));
        });
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpLocalizationOptions>(options =>
        {
            options.GlobalContributors.Add<DynamicLocalizationResourceContributor>();
        });

        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("X.Abp.LanguageManagement", typeof(LanguageManagementResource));
        });

        context.Services.AddAutoMapperObjectMapper<AbpLanguageManagementDomainModule>();

        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddProfile<AbpLanguageManagementDomainAutoMapperProfile>(validate: true);
        });

        Configure<AbpDistributedEntityEventOptions>(options =>
        {
            options.EtoMappings.Add<Language, LanguageEto>(typeof(AbpLanguageManagementDomainModule));
            options.EtoMappings.Add<LanguageText, LanguageTextEto>(typeof(AbpLanguageManagementDomainModule));
        });

        if (context.Services.IsDataMigrationEnvironment())
        {
            Configure<AbpExternalLocalizationOptions>(options => options.SaveToExternalStore = false);
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        await SaveLocalizationAsync(context);
    }

    public override Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        cancellationTokenSource.CancelAsync();
        return Task.CompletedTask;
    }

    private async Task SaveLocalizationAsync(ApplicationInitializationContext applicationInitializationContext)
    {
        if (applicationInitializationContext.ServiceProvider.GetRequiredService<IOptions<AbpExternalLocalizationOptions>>().Value.SaveToExternalStore)
        {
            IRootServiceProvider rootServiceProvider = applicationInitializationContext.ServiceProvider.GetRequiredService<IRootServiceProvider>();
            await Task.Run(async () =>
            {
                using (IServiceScope scope = rootServiceProvider.CreateScope())
                {
                    var applicationLifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
                    var cancellationTokenProvider = scope.ServiceProvider.GetRequiredService<ICancellationTokenProvider>();
                    var cancellationToken = applicationLifetime?.ApplicationStopping ?? cancellationTokenSource.Token;
                    try
                    {
                        using (cancellationTokenProvider.Use(cancellationToken))
                        {
                            if (cancellationTokenProvider.Token.IsCancellationRequested)
                            {
                                return;
                            }

                            await Policy.Handle<Exception>().WaitAndRetryAsync(8, retryAttempt => TimeSpan.FromSeconds(RandomHelper.GetRandom((int)Math.Pow(2.0, retryAttempt) * 8, (int)Math.Pow(2.0, retryAttempt) * 12)))
                            .ExecuteAsync(async _ =>
                            {
                                try
                                {
                                    await scope.ServiceProvider.GetRequiredService<IExternalLocalizationSaver>().SaveAsync();
                                }
                                catch (Exception ex)
                                {
                                    ILogger<AbpLocalizationModule> logger = scope.ServiceProvider.GetService<ILogger<AbpLocalizationModule>>();
                                    if (logger != null)
                                    {
                                        logger.LogException(ex);
                                    }

                                    throw;
                                }
                            },
                            cancellationTokenProvider.Token);
                        }
                    }
                    catch
                    {
                    }
                }
            });
        }
    }
}
