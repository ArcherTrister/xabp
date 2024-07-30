// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Identity;

public class IdentityPasswordSettingsDto
{
    /// <summary>
    /// 密码的最小长度
    /// </summary>
    [Range(2, 128)]
    [Display(Name = "Abp.Identity.Password.RequiredLength")]
    public int RequiredLength { get; set; }

    /// <summary>
    /// 密码必须包含唯一字符的数量
    /// </summary>
    [Range(1, 128)]
    [Display(Name = "Abp.Identity.Password.RequiredUniqueChars")]
    public int RequiredUniqueChars { get; set; }

    /// <summary>
    /// 密码是否必须包含非字母数字
    /// </summary>
    public bool RequireNonAlphanumeric { get; set; }

    /// <summary>
    /// 密码是否必须包含小写字母
    /// </summary>
    public bool RequireLowercase { get; set; }

    /// <summary>
    /// 密码是否必须包含大写字母
    /// </summary>
    public bool RequireUppercase { get; set; }

    /// <summary>
    /// 密码是否必须包含数字
    /// </summary>
    public bool RequireDigit { get; set; }
}
