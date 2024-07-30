// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Identity;

public class IdentityUserSettingsDto
{
    /// <summary>
    /// 是否允许用户更新用户名
    /// </summary>
    public bool IsUserNameUpdateEnabled { get; set; }

    /// <summary>
    /// 是否允许用户更新电子邮箱
    /// </summary>
    public bool IsEmailUpdateEnabled { get; set; }
}
