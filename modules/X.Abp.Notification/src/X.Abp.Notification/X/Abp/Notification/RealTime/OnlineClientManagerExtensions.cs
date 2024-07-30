// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Linq;
using System.Threading.Tasks;

using JetBrains.Annotations;

using Volo.Abp;

namespace X.Abp.Notification.RealTime
{
    /// <summary>
    /// Extension methods for <see cref="IOnlineClientManager"/>.
    /// </summary>
    public static class OnlineClientManagerExtensions
    {
        /// <summary>
        /// Determines whether the specified user is online or not.
        /// </summary>
        /// <param name="onlineClientManager">The online client manager.</param>
        /// <param name="user">User.</param>
        public static async Task<bool> IsOnlineAsync(
            [NotNull] this IOnlineClientManager onlineClientManager,
            [NotNull] UserIdentifier user)
        {
            return (await onlineClientManager.GetAllByUserIdAsync(user)).Any();
        }

        public static async Task<bool> RemoveAsync(
            [NotNull] this IOnlineClientManager onlineClientManager,
            [NotNull] IOnlineClient client)
        {
            Check.NotNull(onlineClientManager, nameof(onlineClientManager));
            Check.NotNull(client, nameof(client));

            return await onlineClientManager.RemoveAsync(client.ConnectionId);
        }
    }
}
