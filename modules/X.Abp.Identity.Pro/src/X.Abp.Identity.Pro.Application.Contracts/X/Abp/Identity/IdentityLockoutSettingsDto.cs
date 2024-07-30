// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Identity;

public class IdentityLockoutSettingsDto
{
    /// <summary>
    /// 允许新用户被锁定
    /// </summary>
    public bool AllowedForNewUsers { get; set; }

    /// <summary>
    /// 当锁定发生时用户被的锁定的时间(秒)
    /// </summary>
    [Display(Name = "Abp.Identity.Lockout.LockoutDuration")]
    public int LockoutDuration { get; set; }

    /// <summary>
    /// 如果启用锁定, 当用户被锁定前失败的访问尝试次数
    /// </summary>
    [Display(Name = "Abp.Identity.Lockout.MaxFailedAccessAttempts")]
    public int MaxFailedAccessAttempts { get; set; }
}
