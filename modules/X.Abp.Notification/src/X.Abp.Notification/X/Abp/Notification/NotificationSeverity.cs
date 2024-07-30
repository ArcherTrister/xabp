// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Notification
{
    /// <summary>
    /// Notification severity.
    /// </summary>
    public enum NotificationSeverity : byte
    {
        /// <summary>
        /// Info.
        /// </summary>
        Info = 0,

        /// <summary>
        /// Success.
        /// </summary>
        Success = 1,

        /// <summary>
        /// Warn.
        /// </summary>
        Warn = 2,

        /// <summary>
        /// Error.
        /// </summary>
        Error = 3,

        /// <summary>
        /// Fatal.
        /// </summary>
        Fatal = 4
    }
}
