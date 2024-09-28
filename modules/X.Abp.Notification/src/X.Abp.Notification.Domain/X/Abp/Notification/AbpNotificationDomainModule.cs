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
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain;
using Volo.Abp.Localization.ExceptionHandling;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

using X.Abp.Notification.Localization;

namespace X.Abp.Notification;

[DependsOn(
    typeof(AbpBackgroundJobsAbstractionsModule),
    typeof(AbpCachingModule),
    typeof(AbpDddDomainModule),
    typeof(AbpNotificationDomainSharedModule))]
#pragma warning disable CA1001 // 具有可释放字段的类型应该是可释放的
public class AbpNotificationDomainModule : AbpModule
#pragma warning restore CA1001 // 具有可释放字段的类型应该是可释放的
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private Task _initializeDynamicNotificationsTask;

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpExceptionLocalizationOptions>(options =>
        {
            options.MapCodeNamespace("AbpNotification", typeof(AbpNotificationResource));
        });

        if (context.Services.IsDataMigrationEnvironment())
        {
            Configure<AbpNotificationOptions>(options =>
            {
                options.SaveStaticNotificationsToDatabase = false;
                options.IsDynamicNotificationStoreEnabled = false;
            });
        }
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        AsyncHelper.RunSync(() => OnApplicationInitializationAsync(context));
    }

    public override Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        InitializeDynamicNotifications(context);
        return Task.CompletedTask;
    }

    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        _cancellationTokenSource.Cancel();
    }

    public override async Task OnApplicationShutdownAsync(ApplicationShutdownContext context)
    {
        await _cancellationTokenSource.CancelAsync();
    }

    public virtual Task GetInitializeDynamicNotificationsTask()
    {
        return _initializeDynamicNotificationsTask ?? Task.CompletedTask;
    }

    private void InitializeDynamicNotifications(ApplicationInitializationContext context)
    {
        var options = context
            .ServiceProvider
            .GetRequiredService<IOptions<AbpNotificationOptions>>()
            .Value;

        if (!options.SaveStaticNotificationsToDatabase && !options.IsDynamicNotificationStoreEnabled)
        {
            return;
        }

        var rootServiceProvider = context.ServiceProvider.GetRequiredService<IRootServiceProvider>();

        _initializeDynamicNotificationsTask = Task.Run(async () =>
        {
            using var scope = rootServiceProvider.CreateScope();
            var applicationLifetime = scope.ServiceProvider.GetService<IHostApplicationLifetime>();
            var cancellationTokenProvider = scope.ServiceProvider.GetRequiredService<ICancellationTokenProvider>();
            var cancellationToken = applicationLifetime?.ApplicationStopping ?? _cancellationTokenSource.Token;

            try
            {
                using (cancellationTokenProvider.Use(cancellationToken))
                {
                    if (cancellationTokenProvider.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    await SaveStaticNotificationsToDatabaseAsync(options, scope, cancellationTokenProvider);

                    if (cancellationTokenProvider.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    await PreCacheDynamicNotificationsAsync(options, scope);
                }
            }

            // ReSharper disable once EmptyGeneralCatchClause (No need to log since it is logged above)
            catch
            {
            }
        });
    }

    private static async Task SaveStaticNotificationsToDatabaseAsync(
        AbpNotificationOptions options,
        IServiceScope scope,
        ICancellationTokenProvider cancellationTokenProvider)
    {
        if (!options.SaveStaticNotificationsToDatabase)
        {
            return;
        }

        await Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                8,
                retryAttempt => TimeSpan.FromSeconds(
                    RandomHelper.GetRandom(
                        (int)Math.Pow(2, retryAttempt) * 8,
                        (int)Math.Pow(2, retryAttempt) * 12)))
            .ExecuteAsync(async _ =>
            {
                try
                {
                    // ReSharper disable once AccessToDisposedClosure
                    await scope
                        .ServiceProvider
                        .GetRequiredService<IStaticNotificationSaver>()
                        .SaveAsync();
                }
                catch (Exception ex)
                {
                    // ReSharper disable once AccessToDisposedClosure
                    scope.ServiceProvider
                        .GetService<ILogger<AbpNotificationDomainModule>>()?
                        .LogException(ex);

                    throw; // Polly will catch it
                }
            },
            cancellationToken: cancellationTokenProvider.Token);
    }

    private static async Task PreCacheDynamicNotificationsAsync(AbpNotificationOptions options, IServiceScope scope)
    {
        if (!options.IsDynamicNotificationStoreEnabled)
        {
            return;
        }

        try
        {
            // Pre-cache notifications, so first request doesn't wait
            await scope
                .ServiceProvider
                .GetRequiredService<IDynamicNotificationDefinitionStore>()
                .GetGroupsAsync();
        }
        catch (Exception ex)
        {
            // ReSharper disable once AccessToDisposedClosure
            scope
                .ServiceProvider
                .GetService<ILogger<AbpNotificationDomainModule>>()?
                .LogException(ex);

            throw; // It will be cached in InitializeDynamicNotifications
        }
    }
}
