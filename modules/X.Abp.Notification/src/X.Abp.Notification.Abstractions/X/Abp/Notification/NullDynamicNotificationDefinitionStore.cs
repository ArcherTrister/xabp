﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification;

public class NullDynamicNotificationDefinitionStore : IDynamicNotificationDefinitionStore, ISingletonDependency
{
  private static readonly Task<NotificationDefinition> CachedNotificationResult = Task.FromResult((NotificationDefinition)null);

  private static readonly Task<IReadOnlyList<NotificationDefinition>> CachedNotificationsResult =
      Task.FromResult((IReadOnlyList<NotificationDefinition>)[]);

  private static readonly Task<IReadOnlyList<NotificationGroupDefinition>> CachedGroupsResult =
      Task.FromResult((IReadOnlyList<NotificationGroupDefinition>)[]);

  public virtual Task<NotificationDefinition> GetOrNullAsync(string name)
  {
    return CachedNotificationResult;
  }

  public virtual Task<IReadOnlyList<NotificationDefinition>> GetNotificationsAsync()
  {
    return CachedNotificationsResult;
  }

  public virtual Task<IReadOnlyList<NotificationGroupDefinition>> GetGroupsAsync()
  {
    return CachedGroupsResult;
  }
}
