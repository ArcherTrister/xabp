// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Settings;
using Volo.Abp.Threading;

namespace X.Abp.AuditLogging
{
    [BackgroundWorkerName("X.Abp.AuditLogging.ExpiredAuditLogDeleterWorker")]
    public class ExpiredAuditLogDeleterWorker : AsyncPeriodicBackgroundWorkerBase
    {
        public ExpiredAuditLogDeleterWorker(
          AbpAsyncTimer timer,
          IServiceScopeFactory serviceScopeFactory,
          IOptions<ExpiredAuditLogDeleterOptions> options)
          : base(timer, serviceScopeFactory)
        {
            timer.Period = options.Value.Period;
        }

        protected override async Task DoWorkAsync(PeriodicBackgroundWorkerContext workerContext)
        {
            if (!await workerContext.ServiceProvider.GetRequiredService<ISettingProvider>().IsTrueAsync(AuditLogSettingNames.IsPeriodicDeleterEnabled))
            {
                return;
            }

            Logger.LogInformation("Expired audit log deleter worker started");
            await workerContext.ServiceProvider.GetRequiredService<ExpiredAuditLogDeleterService>().DeleteAsync();
            Logger.LogInformation("Expired audit log deleter worker finished");
        }
    }
}
