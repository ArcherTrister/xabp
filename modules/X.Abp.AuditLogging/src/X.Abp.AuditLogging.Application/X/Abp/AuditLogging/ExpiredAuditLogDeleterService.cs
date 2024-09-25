// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using Volo.Abp.AuditLogging;
using Volo.Abp.DependencyInjection;
using Volo.Abp.DistributedLocking;
using Volo.Abp.Features;
using Volo.Abp.MultiTenancy;
using Volo.Abp.SettingManagement;
using Volo.Abp.Settings;
using Volo.Abp.Threading;
using Volo.Abp.Timing;

using X.Abp.AuditLogging.Features;

namespace X.Abp.AuditLogging
{
    public class ExpiredAuditLogDeleterService : ITransientDependency
    {
        private bool globalIsExpiredDeleterEnabled;
        private int globalExpiredDeleterPeriod;

        public ILogger<ExpiredAuditLogDeleterService> Logger { get; set; }

        protected IAbpDistributedLock DistributedLock { get; }

        protected ITenantStore TenantStore { get; }

        protected ISettingProvider SettingProvider { get; }

        protected ISettingManager SettingManager { get; }

        protected IFeatureChecker FeatureChecker { get; }

        protected IAuditLogRepository AuditLogRepository { get; }

        protected ICurrentTenant CurrentTenant { get; }

        protected IClock Clock { get; }

        protected ICancellationTokenProvider CancellationTokenProvider { get; }

        public ExpiredAuditLogDeleterService(
          IAbpDistributedLock distributedLock,
          ITenantStore tenantStore,
          ISettingProvider settingProvider,
          ISettingManager settingManager,
          IFeatureChecker featureChecker,
          IAuditLogRepository auditLogRepository,
          ICurrentTenant currentTenant,
          IClock clock,
          ICancellationTokenProvider cancellationTokenProvider)
        {
            Logger = NullLogger<ExpiredAuditLogDeleterService>.Instance;
            DistributedLock = distributedLock;
            TenantStore = tenantStore;
            SettingProvider = settingProvider;
            SettingManager = settingManager;
            FeatureChecker = featureChecker;
            AuditLogRepository = auditLogRepository;
            CurrentTenant = currentTenant;
            Clock = clock;
            CancellationTokenProvider = cancellationTokenProvider;
        }

        public virtual async Task DeleteAsync()
        {
            IAbpDistributedLockHandle handle = await DistributedLock.TryAcquireAsync("ExpiredAuditLogDeleterLock");

            try
            {
                if (handle == null)
                {
                    Logger.LogInformation("Could not acquire distributed lock for {ExpiredAuditLogDeleterService}!", typeof(ExpiredAuditLogDeleterService));
                }
                else
                {
                    Logger.LogInformation("Expired audit log deletion is started. Lock is acquired for {ExpiredAuditLogDeleterService}", typeof(ExpiredAuditLogDeleterService));
                    await GetAuditLogGlobalSettingsAsync();
                    using (CurrentTenant.Change(null))
                    {
                        await DeleteExpiredAuditLogsAsync();
                    }

                    foreach (TenantConfiguration tenantConfiguration in await TenantStore.GetListAsync(false))
                    {
                        CancellationTokenProvider.Token.ThrowIfCancellationRequested();
                        using (CurrentTenant.Change(tenantConfiguration.Id))
                        {
                            await DeleteExpiredAuditLogsAsync();
                        }
                    }

                    Logger.LogInformation("Expired audit log deletion is completed. Lock is released for {ExpiredAuditLogDeleterService}", typeof(ExpiredAuditLogDeleterService));
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

        private async Task GetAuditLogGlobalSettingsAsync()
        {
            globalIsExpiredDeleterEnabled = (await SettingManager.GetOrNullGlobalAsync(AuditLogSettingNames.IsExpiredDeleterEnabled))?.To<bool>() ?? false;
            if (!globalIsExpiredDeleterEnabled)
            {
                return;
            }

            globalExpiredDeleterPeriod = (await SettingManager.GetOrNullGlobalAsync(AuditLogSettingNames.ExpiredDeleterPeriod))?.To<int>() ?? 0;
        }

        protected virtual async Task DeleteExpiredAuditLogsAsync()
        {
            bool isFeatureEnable = await FeatureChecker.IsEnabledAsync(AbpAuditLoggingFeatures.SettingManagement);
            bool isExpiredDeleterEnabled;
            if (!isFeatureEnable)
            {
                isExpiredDeleterEnabled = !globalIsExpiredDeleterEnabled;
            }
            else
            {
                isExpiredDeleterEnabled = !await SettingProvider.IsTrueAsync(AuditLogSettingNames.IsExpiredDeleterEnabled);
            }

            if (isExpiredDeleterEnabled)
            {
                Logger.LogInformation("The clean up service for tenant: {CurrentTenantId} isn't enabled", CurrentTenant.Id);
            }
            else
            {
                int expiredDeleterPeriod;
                if (!isFeatureEnable)
                {
                    expiredDeleterPeriod = globalExpiredDeleterPeriod;
                }
                else
                {
                    expiredDeleterPeriod = await SettingProvider.GetAsync<int>(AuditLogSettingNames.ExpiredDeleterPeriod);
                }

                DateTime minDate = Clock.Now.Date.AddDays(-expiredDeleterPeriod);
                try
                {
                    await AuditLogRepository.DeleteDirectAsync(auditLog => auditLog.TenantId == CurrentTenant.Id && auditLog.ExecutionTime < minDate);
                    Logger.LogInformation("Expired audit logs have been successfully deleted for tenant: {CurrentTenantId}, covering dates up to less than {Date}", CurrentTenant.Id, minDate);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Failed to delete expired audit logs for tenant: {CurrentTenantId}", CurrentTenant.Id);
                }
            }
        }
    }
}
