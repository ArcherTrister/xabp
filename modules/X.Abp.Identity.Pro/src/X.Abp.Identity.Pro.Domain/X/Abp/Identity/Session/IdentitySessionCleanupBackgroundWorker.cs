// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.BackgroundWorkers;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Threading;

namespace X.Abp.Identity.Session;

[BackgroundWorkerName("X.Abp.Identity.Session.IdentitySessionCleanupBackgroundWorker")]
public class IdentitySessionCleanupBackgroundWorker : AsyncPeriodicBackgroundWorkerBase
{
    protected IAbpDistributedLock DistributedLock { get; }

    public IdentitySessionCleanupBackgroundWorker(
      AbpAsyncTimer timer,
      IServiceScopeFactory serviceScopeFactory,
      IOptionsMonitor<IdentitySessionCleanupOptions> cleanupOptions,
      IAbpDistributedLock distributedLock)
      : base(timer, serviceScopeFactory)
    {
        DistributedLock = distributedLock;
        Timer.Period = cleanupOptions.CurrentValue.CleanupPeriod;
    }

    protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
    {
        IAbpDistributedLockHandle handle = await DistributedLock.TryAcquireAsync("IdentitySessionCleanupLock");
        try
        {
            Logger.LogInformation("Lock is acquired for {IdentitySessionCleanupBackgroundWorker}", typeof(IdentitySessionCleanupBackgroundWorker));
            if (handle == null)
            {
                Logger.LogInformation("Could not acquire distributed lock for {IdentitySessionCleanupBackgroundWorker}!", typeof(IdentitySessionCleanupBackgroundWorker));
            }
            else
            {
                await workerContext.ServiceProvider.GetRequiredService<IdentitySessionCleanupService>().CleanAsync();
                Logger.LogInformation("Lock is released for {IdentitySessionCleanupBackgroundWorker}", typeof(IdentitySessionCleanupBackgroundWorker));
            }
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            if (handle != null)
            {
                await handle.DisposeAsync();
            }
        }
    }
}
