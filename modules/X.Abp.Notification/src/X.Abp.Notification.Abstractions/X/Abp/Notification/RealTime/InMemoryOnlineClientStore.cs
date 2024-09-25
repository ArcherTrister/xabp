// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification.RealTime
{
  public class InMemoryOnlineClientStore<T> : InMemoryOnlineClientStore, IOnlineClientStore<T>
  {
  }

  public class InMemoryOnlineClientStore : IOnlineClientStore, ISingletonDependency
  {
    /// <summary>
    /// Online clients.
    /// </summary>
    protected ConcurrentDictionary<string, IOnlineClient> Clients { get; }

    public InMemoryOnlineClientStore()
    {
      Clients = new ConcurrentDictionary<string, IOnlineClient>();
    }

    public virtual Task AddAsync(IOnlineClient client)
    {
      Clients.AddOrUpdate(client.ConnectionId, client, (s, o) => client);
      return Task.CompletedTask;
    }

    public virtual Task<bool> RemoveAsync(string connectionId)
    {
      return TryRemoveAsync(connectionId, value => _ = value);
    }

    public virtual Task<bool> TryRemoveAsync(string connectionId, Action<IOnlineClient> clientAction)
    {
      var hasRemoved = Clients.TryRemove(connectionId, out IOnlineClient client);
      clientAction(client);
      return Task.FromResult(hasRemoved);
    }

    public virtual Task<bool> TryGetAsync(string connectionId, Action<IOnlineClient> clientAction)
    {
      var hasValue = Clients.TryGetValue(connectionId, out IOnlineClient client);
      clientAction(client);
      return Task.FromResult(hasValue);
    }

    public virtual Task<bool> ContainsAsync(string connectionId)
    {
      var hasKey = Clients.TryGetValue(connectionId, out var _);
      return Task.FromResult(hasKey);
    }

    public virtual Task<IReadOnlyList<IOnlineClient>> GetAllAsync()
    {
      return Task.FromResult<IReadOnlyList<IOnlineClient>>(Clients.Values.ToImmutableList());
    }

    public virtual async Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync(UserIdentifier userIdentifier)
    {
      return (await GetAllAsync())
          .Where(c => c.UserId == userIdentifier.UserId && c.TenantId == userIdentifier.TenantId)
          .ToImmutableList();
    }
  }
}
