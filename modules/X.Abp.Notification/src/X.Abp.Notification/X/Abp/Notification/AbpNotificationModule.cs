// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Volo.Abp.Json;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Timing;
using Volo.Abp.VirtualFileSystem;

using X.Abp.Notification.Localization;
using X.Abp.Notification.RealTime;

namespace X.Abp.Notification;

[DependsOn(
    typeof(AbpJsonAbstractionsModule),
    typeof(AbpLocalizationModule),
    typeof(AbpMultiTenancyModule),
    typeof(AbpTimingModule))]
public class AbpNotificationModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AutoAddDefinitionProviders(context.Services);
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AbpNotificationResource>();
        });

        Configure<AbpLocalizationOptions>(options =>
        {
            options.Resources
                .Add<AbpNotificationResource>("en");
        });

        context.Services.Configure<AbpNotificationOptions>(options =>
        {
            options.Notifiers.Add<NullRealTimeNotifier>();
        });
    }

    private static void AutoAddDefinitionProviders(IServiceCollection services)
    {
        var definitionProviders = new List<Type>();

        services.OnRegistered(context =>
        {
            if (typeof(INotificationDefinitionProvider).IsAssignableFrom(context.ImplementationType))
            {
                definitionProviders.Add(context.ImplementationType);
            }
        });

        services.Configure<AbpNotificationOptions>(options =>
        {
            options.DefinitionProviders.AddIfNotContains(definitionProviders);
        });
    }
}
