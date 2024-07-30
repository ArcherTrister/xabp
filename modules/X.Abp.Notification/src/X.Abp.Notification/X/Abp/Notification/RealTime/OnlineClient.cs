// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp.MultiTenancy;

namespace X.Abp.Notification.RealTime
{
    /// <summary>
    /// Implements <see cref="IOnlineClient"/>.
    /// </summary>
    [Serializable]
    public class OnlineClient : IOnlineClient, IMultiTenant
    {
        /// <summary>
        /// Unique connection Id for this client.
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// IP address of this client.
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Tenant Id.
        /// </summary>
        public Guid? TenantId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Connection establishment time for this client.
        /// </summary>
        public DateTime ConnectTime { get; set; }

        /// <summary>
        /// Shortcut to set/get <see cref="Properties"/>.
        /// </summary>
        public object this[string key]
        {
            get { return Properties[key]; }
            set { Properties[key] = value; }
        }

        /// <summary>
        /// Can be used to add custom properties for this client.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get
            {
                return _properties;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _properties = value;
            }
        }

        private Dictionary<string, object> _properties;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClient"/> class.
        /// </summary>
        public OnlineClient()
        {
            ConnectTime = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClient"/> class.
        /// </summary>
        /// <param name="connectionId">The connection identifier.</param>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="userId">The user identifier.</param>
        public OnlineClient(string connectionId, string ipAddress, Guid? tenantId, Guid? userId)
            : this()
        {
            ConnectionId = connectionId;
            IpAddress = ipAddress;
            TenantId = tenantId;
            UserId = userId;

            Properties = new Dictionary<string, object>();
        }

        //public override string ToString()
        //{
        //    return this.ToJsonString();
        //}
    }
}
