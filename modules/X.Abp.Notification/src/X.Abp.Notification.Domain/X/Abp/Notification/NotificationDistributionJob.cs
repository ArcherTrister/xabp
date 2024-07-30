// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Threading.Tasks;

using Volo.Abp.BackgroundJobs;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification
{
    /// <summary>
    /// This background job distributes notifications to users.
    /// </summary>
    public class NotificationDistributionJob : IAsyncBackgroundJob<NotificationDistributionJobArgs>, ITransientDependency
    {
        protected INotificationDistributer NotificationDistributer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDistributionJob"/> class.
        /// </summary>
        public NotificationDistributionJob(INotificationDistributer notificationDistributer)
        {
            NotificationDistributer = notificationDistributer;
        }

        public async Task ExecuteAsync(NotificationDistributionJobArgs args)
        {
            await NotificationDistributer.DistributeAsync(args.NotificationId);
        }
    }
}
