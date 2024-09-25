// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using JetBrains.Annotations;

namespace X.Abp.Notification.RealTime
{
    /// <summary>
    /// Used to manage online clients those are connected to the application.
    /// </summary>
    public interface IOnlineClientManager<T> : IOnlineClientManager
    {
    }

    public interface IOnlineClientManager
    {
        event EventHandler<OnlineClientEventArgs> ClientConnected;

        event EventHandler<OnlineClientEventArgs> ClientDisconnected;

        event EventHandler<OnlineUserEventArgs> UserConnected;

        event EventHandler<OnlineUserEventArgs> UserDisconnected;

        /// <summary>
        /// Adds a client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>The task to handle async operation</returns>
        Task AddAsync(IOnlineClient client);

        /// <summary>
        /// Removes a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <returns>True, if a client is removed</returns>
        Task<bool> RemoveAsync(string connectionId);

        /// <summary>
        /// Tries to find a client by connection id.
        /// Returns null if not found.
        /// </summary>
        /// <param name="connectionId">connection id</param>
        Task<IOnlineClient> GetByConnectionIdOrNullAsync(string connectionId);

        /// <summary>
        /// Gets all online clients asynchronously.
        /// </summary>
        Task<IReadOnlyList<IOnlineClient>> GetAllClientsAsync();

        /// <summary>
        /// Gets all online clients by user id asynchronously.
        /// </summary>
        /// <param name="user">user identifier</param>
        Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync([NotNull] IUserIdentifier user);
    }
}
