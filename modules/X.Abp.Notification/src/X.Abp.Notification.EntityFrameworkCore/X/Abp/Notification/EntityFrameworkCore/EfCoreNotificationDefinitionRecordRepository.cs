// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.Notification.EntityFrameworkCore;

public class EfCoreNotificationDefinitionRecordRepository :
    EfCoreRepository<INotificationDbContext, NotificationDefinitionRecord, Guid>,
    INotificationDefinitionRecordRepository
{
    public EfCoreNotificationDefinitionRecordRepository(
        IDbContextProvider<INotificationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    public virtual async Task<NotificationDefinitionRecord> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await (await GetDbSetAsync())
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(r => r.Name == name, cancellationToken);
    }
}
