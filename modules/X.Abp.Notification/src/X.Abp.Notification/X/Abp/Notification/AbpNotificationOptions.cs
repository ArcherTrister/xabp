// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

using Volo.Abp.Collections;

using X.Abp.Notification.RealTime;

namespace X.Abp.Notification;

public class AbpNotificationOptions
{
    public ITypeList<INotificationDefinitionProvider> DefinitionProviders { get; }

    /// <summary>
    /// A list of contributors for notification notifying process.
    /// </summary>
    public ITypeList<IRealTimeNotifier> Notifiers { get; }

    public HashSet<string> DeletedNotifications { get; }

    public HashSet<string> DeletedNotificationGroups { get; }

    public Dictionary<string, string> ProviderPolicies { get; }

    /// <summary>
    /// Default: true.
    /// </summary>
    public bool SaveStaticNotificationsToDatabase { get; set; } = true;

    /// <summary>
    /// Default: false.
    /// </summary>
    public bool IsDynamicNotificationStoreEnabled { get; set; }

    public AbpNotificationOptions()
    {
        DefinitionProviders = new TypeList<INotificationDefinitionProvider>();
        Notifiers = new TypeList<IRealTimeNotifier>();

        DeletedNotifications = new HashSet<string>();
        DeletedNotificationGroups = new HashSet<string>();

        ProviderPolicies = new Dictionary<string, string>();
    }
}
