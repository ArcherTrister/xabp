// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Identity;

public class IdentitySignInSettingsDto
{
    /// <summary>
    /// 登录时是否需要验证电子邮箱
    /// </summary>
    public bool RequireConfirmedEmail { get; set; }

    /// <summary>
    /// 用户是否可以确认手机号码
    /// </summary>
    public bool EnablePhoneNumberConfirmation { get; set; }

    /// <summary>
    /// 登录时是否需要验证手机号码
    /// </summary>
    public bool RequireConfirmedPhoneNumber { get; set; }
}
