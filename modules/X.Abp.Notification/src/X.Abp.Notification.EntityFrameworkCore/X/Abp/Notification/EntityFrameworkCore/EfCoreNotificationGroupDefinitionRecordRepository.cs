// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.Notification.EntityFrameworkCore;

public class EfCoreNotificationGroupDefinitionRecordRepository :
    EfCoreRepository<INotificationDbContext, NotificationGroupDefinitionRecord, Guid>,
    INotificationGroupDefinitionRecordRepository
{
    public EfCoreNotificationGroupDefinitionRecordRepository(
        IDbContextProvider<INotificationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }
}
