// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Caching;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace X.Abp.Chat;

[DependsOn(
    typeof(AbpChatApplicationContractsModule),
    typeof(AbpAspNetCoreSignalRModule),
    typeof(AbpEventBusModule),
    typeof(AbpCachingModule))]
public class AbpChatSignalRModule : AbpModule
{
}
