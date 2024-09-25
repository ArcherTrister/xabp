// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using EFCore.BulkExtensions;

using Volo.Abp.Auditing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace X.Abp.EntityFrameworkCore;

/// <summary>
/// 批量数据操作使用EFCore.BulkExtensions库实现
/// https://github.com/abpframework/abp/issues/18094
/// </summary>
public class BulkExtensionsSqliteEfCoreBulkOperationProvider
    : IEfCoreBulkOperationProvider, ITransientDependency
{
  protected IAuditPropertySetter AuditPropertySetter { get; }

  public BulkExtensionsSqliteEfCoreBulkOperationProvider(IAuditPropertySetter auditPropertySetter)
  {
    AuditPropertySetter = auditPropertySetter;
  }

  public virtual async Task DeleteManyAsync<TDbContext, TEntity>(
      IEfCoreRepository<TEntity> repository,
      IEnumerable<TEntity> entities,
      bool autoSave,
      CancellationToken cancellationToken)
      where TDbContext : IEfCoreDbContext
      where TEntity : class, IEntity
  {
    var entityList = (List<TEntity>)entities;
    foreach (TEntity entity in entityList)
    {
      AuditPropertySetter.SetDeletionProperties(entity);
    }

    var dbContext = await repository.GetDbContextAsync();
    await dbContext.BulkDeleteAsync(entityList, cancellationToken: cancellationToken);
    if (autoSave)
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
  }

  public virtual async Task InsertManyAsync<TDbContext, TEntity>(
      IEfCoreRepository<TEntity> repository,
      IEnumerable<TEntity> entities,
      bool autoSave,
      CancellationToken cancellationToken)
      where TDbContext : IEfCoreDbContext
      where TEntity : class, IEntity
  {
    var entityList = (List<TEntity>)entities;
    foreach (TEntity entity in entityList)
    {
      AuditPropertySetter.SetCreationProperties(entity);
    }

    var dbContext = await repository.GetDbContextAsync();
    await dbContext.BulkInsertAsync(entityList, cancellationToken: cancellationToken);
    if (autoSave)
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
  }

  public virtual async Task UpdateManyAsync<TDbContext, TEntity>(
      IEfCoreRepository<TEntity> repository,
      IEnumerable<TEntity> entities,
      bool autoSave,
      CancellationToken cancellationToken)
      where TDbContext : IEfCoreDbContext
      where TEntity : class, IEntity
  {
    var entityList = (List<TEntity>)entities;
    foreach (TEntity entity in entityList)
    {
      AuditPropertySetter.SetModificationProperties(entity);
    }

    var dbContext = await repository.GetDbContextAsync();
    await dbContext.BulkUpdateAsync(entityList, cancellationToken: cancellationToken);
    if (autoSave)
    {
      await dbContext.SaveChangesAsync(cancellationToken);
    }
  }
}
