// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

namespace X.Abp.Notification
{
    /// <summary>
    /// Used to store data for a notification.
    /// It can be directly used or can be derived.
    /// </summary>
    [Serializable]
    public class NotificationData
    {
        /// <summary>
        /// Gets notification data type name.
        /// It returns the full class name by default.
        /// </summary>
        public virtual string Type => GetType().FullName;

        /// <summary>
        /// Shortcut to set/get <see cref="Properties"/>.
        /// </summary>
        public object this[string key]
        {
            get { return Properties.GetOrDefault(key); }
            set { Properties[key] = value; }
        }

        /// <summary>
        /// Can be used to add custom properties to this notification.
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

                /* Not assign value, but add dictionary items. This is required for backward compability. */
                foreach (var keyValue in value)
                {
                    if (!_properties.TryGetValue(keyValue.Key, out var _))
                    {
                        _properties[keyValue.Key] = keyValue.Value;
                    }
                }
            }
        }

        private readonly Dictionary<string, object> _properties;

        /// <summary>
        /// Createa a new NotificationData object.
        /// </summary>
        public NotificationData()
        {
            _properties = new Dictionary<string, object>();
        }

/*        public override string ToString()
        {
            return this.ToJsonString();
        }*/
    }
}
