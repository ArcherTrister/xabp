// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

namespace X.Abp.Notification.RealTime
{
    /// <summary>
    /// Represents an online client connected to the application.
    /// </summary>
    public interface IOnlineClient
    {
        /// <summary>
        /// Unique connection Id for this client.
        /// </summary>
        string ConnectionId { get; }

        /// <summary>
        /// IP address of this client.
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// Tenant Id.
        /// </summary>
        Guid? TenantId { get; }

        /// <summary>
        /// User Id.
        /// </summary>
        Guid? UserId { get; }

        /// <summary>
        /// Connection establishment time for this client.
        /// </summary>
        DateTime ConnectTime { get; }

        /// <summary>
        /// Shortcut to set/get <see cref="Properties"/>.
        /// </summary>
        object this[string key] { get; set; }

        /// <summary>
        /// Can be used to add custom properties for this client.
        /// </summary>
        Dictionary<string, object> Properties { get; }
    }
}
