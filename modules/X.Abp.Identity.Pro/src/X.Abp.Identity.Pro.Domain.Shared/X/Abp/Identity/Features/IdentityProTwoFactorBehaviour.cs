// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Identity.Features;

public enum IdentityProTwoFactorBehaviour
{
    /// <summary>
    /// 可选
    /// </summary>
    Optional,

    /// <summary>
    /// 禁用
    /// </summary>
    Disabled,

    /// <summary>
    /// 强制启用
    /// </summary>
    Forced
}
