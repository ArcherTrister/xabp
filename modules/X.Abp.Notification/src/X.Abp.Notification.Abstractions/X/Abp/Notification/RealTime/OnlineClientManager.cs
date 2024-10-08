﻿// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace X.Abp.Notification.RealTime
{
  /// <summary>
  /// Implements <see cref="IOnlineClientManager"/>.
  /// </summary>
  public class OnlineClientManager : IOnlineClientManager, ISingletonDependency
  {
    public event EventHandler<OnlineClientEventArgs> ClientConnected;

    public event EventHandler<OnlineClientEventArgs> ClientDisconnected;

    public event EventHandler<OnlineUserEventArgs> UserConnected;

    public event EventHandler<OnlineUserEventArgs> UserDisconnected;

    /// <summary>
    /// Online clients Store.
    /// </summary>
    protected IOnlineClientStore Store { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OnlineClientManager"/> class.
    /// </summary>
    public OnlineClientManager(IOnlineClientStore store)
    {
      Store = store;
    }

    public virtual async Task AddAsync(IOnlineClient client)
    {
      var userWasAlreadyOnline = false;
      var user = client.ToUserIdentifierOrNull();

      if (user != null)
      {
        userWasAlreadyOnline = await this.IsOnlineAsync(user);
      }

      await Store.AddAsync(client);

      ClientConnected.InvokeSafely(this, new OnlineClientEventArgs(client));

      if (user != null && !userWasAlreadyOnline)
      {
        UserConnected.InvokeSafely(this, new OnlineUserEventArgs(user, client));
      }
    }

    public virtual async Task<bool> RemoveAsync(string connectionId)
    {
      IOnlineClient client = default;
      var result = await Store.TryRemoveAsync(connectionId, value => client = value);
      if (!result)
      {
        return false;
      }

      if (UserDisconnected != null)
      {
        var user = client.ToUserIdentifierOrNull();

        if (user != null && !await this.IsOnlineAsync(user))
        {
          UserDisconnected.InvokeSafely(this, new OnlineUserEventArgs(user, client));
        }
      }

      ClientDisconnected?.InvokeSafely(this, new OnlineClientEventArgs(client));

      return true;
    }

    public virtual async Task<IOnlineClient> GetByConnectionIdOrNullAsync(string connectionId)
    {
      IOnlineClient client = default;
      if (await Store.TryGetAsync(connectionId, value => client = value))
      {
        return client;
      }

      return null;
    }

    public virtual Task<IReadOnlyList<IOnlineClient>> GetAllClientsAsync()
    {
      return Store.GetAllAsync();
    }

    [NotNull]
    public virtual async Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync([NotNull] IUserIdentifier user)
    {
      Check.NotNull(user, nameof(user));

      var userIdentifier = new UserIdentifier(user.TenantId, user.UserId);
      var clients = await Store.GetAllByUserIdAsync(userIdentifier);

      return clients;
    }
  }
}
