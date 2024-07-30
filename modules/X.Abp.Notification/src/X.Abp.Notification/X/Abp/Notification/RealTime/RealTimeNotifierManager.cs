// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification.RealTime
{
    public class RealTimeNotifierManager : IRealTimeNotifierManager, ISingletonDependency
    {
        public List<IRealTimeNotifier> Notifiers => _lazyNotifiers.Value;

        protected AbpNotificationOptions Options { get; }

        private readonly Lazy<List<IRealTimeNotifier>> _lazyNotifiers;

        public RealTimeNotifierManager(
            IServiceProvider serviceProvider,
            IOptions<AbpNotificationOptions> options)
        {
            Options = options.Value;

            _lazyNotifiers = new Lazy<List<IRealTimeNotifier>>(
                () => Options
                    .Notifiers
                    .Select(type => serviceProvider.GetRequiredService(type) as IRealTimeNotifier)
                    .ToList(),
                true);
        }
    }
}
