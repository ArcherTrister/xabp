// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace X.Abp.VersionManagement.AppEditions;

public enum PublishType
{
    /// <summary>
    /// 待发布
    /// </summary>
    [Description("待发布")]
    [Display(Name = "待发布")]
    ToBeReleased,

    /// <summary>
    /// 已发布
    /// </summary>
    [Description("已发布")]
    [Display(Name = "已发布")]
    Released
}
