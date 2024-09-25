// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Polly;

using Volo.Abp;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;
using Volo.Abp.TextTemplating;
using Volo.Abp.Threading;

using X.Abp.TextTemplateManagement.TextTemplates;

namespace X.Abp.TextTemplateManagement;

[DependsOn(typeof(AbpTextTemplateManagementDomainSharedModule), typeof(AbpTextTemplatingCoreModule), typeof(AbpDddDomainModule), typeof(AbpCachingModule))]
public class AbpTextTemplateManagementDomainModule : AbpModule
{
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        if (!context.Services.IsDataMigrationEnvironment())
        {
            return;
        }

        Configure<TextTemplateManagementOptions>(options =>
        {
            options.SaveStaticTemplatesToDatabase = false;
            options.IsDynamicTemplateStoreEnabled = false;
        });
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        InitializeTextDynamicTemplates(context);
        return Task.CompletedTask;
    }

    public override async Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        await _cancellationTokenSource.CancelAsync();
    }

    private void InitializeTextDynamicTemplates(ApplicationInitializationContext context)
    {
        TextTemplateManagementOptions options = ServiceProviderServiceExtensions.GetRequiredService<IOptions<TextTemplateManagementOptions>>(context.ServiceProvider).Value;
        if (!options.SaveStaticTemplatesToDatabase && !options.IsDynamicTemplateStoreEnabled)
        {
            return;
        }

        IRootServiceProvider rootServiceProvider = ServiceProviderServiceExtensions.GetRequiredService<IRootServiceProvider>(context.ServiceProvider);
        Task.Run(async () =>
        {
            IServiceScope scope = rootServiceProvider.CreateScope();
            try
            {
                IHostApplicationLifetime service = ServiceProviderServiceExtensions.GetService<IHostApplicationLifetime>(scope.ServiceProvider);
                ICancellationTokenProvider cancellationTokenProvider = ServiceProviderServiceExtensions.GetRequiredService<ICancellationTokenProvider>(scope.ServiceProvider);
                CancellationToken cancellationToken = service != null ? service.ApplicationStopping : _cancellationTokenSource.Token;
                try
                {
                    using (cancellationTokenProvider.Use(cancellationToken))
                    {
                        if (cancellationTokenProvider.Token.IsCancellationRequested)
                        {
                            return;
                        }

                        await SaveStaticTemplatesToDatabaseAsync(options, scope, cancellationTokenProvider);
                        if (cancellationTokenProvider.Token.IsCancellationRequested)
                        {
                            return;
                        }

                        await PreCacheDynamicTemplatesAsync(options, scope);
                    }
                }
                catch
                {
                    scope = null;
                    cancellationTokenProvider = null;
                }
            }
            finally
            {
                scope?.Dispose();
            }
        });
    }

    private static async Task SaveStaticTemplatesToDatabaseAsync(
      TextTemplateManagementOptions options,
      IServiceScope scope,
      ICancellationTokenProvider cancellationTokenProvider)
    {
        if (!options.SaveStaticTemplatesToDatabase)
        {
            return;
        }

        await AsyncRetrySyntax.WaitAndRetryAsync(Policy.Handle<Exception>(), 8, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2.0, retryAttempt) * 10.0)).ExecuteAsync(async _ =>
        {
            try
            {
                await ServiceProviderServiceExtensions.GetRequiredService<IStaticTextTemplateSaver>(scope.ServiceProvider).SaveAsync();
            }
            catch (Exception ex)
            {
                ILogger<AbpTextTemplateManagementDomainModule> service = ServiceProviderServiceExtensions.GetService<ILogger<AbpTextTemplateManagementDomainModule>>(scope.ServiceProvider);
                if (service != null)
                {
                    service.LogException(ex);
                }

                throw;
            }
        },
        cancellationTokenProvider.Token);
    }

    private static async Task PreCacheDynamicTemplatesAsync(TextTemplateManagementOptions options, IServiceScope scope)
    {
        if (!options.IsDynamicTemplateStoreEnabled)
        {
            return;
        }

        try
        {
            IReadOnlyList<TemplateDefinition> templateDefinitionList = await ServiceProviderServiceExtensions.GetRequiredService<IDynamicTemplateDefinitionStore>(scope.ServiceProvider).GetAllAsync();
        }
        catch (Exception ex)
        {
            ILogger<AbpTextTemplateManagementDomainModule> service = ServiceProviderServiceExtensions.GetService<ILogger<AbpTextTemplateManagementDomainModule>>(scope.ServiceProvider);
            if (service != null)
            {
                service.LogException(ex);
            }

            throw;
        }
    }
}
