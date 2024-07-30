// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace X.Abp.VersionManagement.AppEditions;

public enum PlatformType
{
    /// <summary>
    /// Android
    /// </summary>
    [Description("android")]
    [Display(Name = "安卓")]
    Android,

    /// <summary>
    /// Windows
    /// </summary>
    [Description("win32")]
    [Display(Name = "Windows")]
    Windows,

    /// <summary>
    /// Mac
    /// </summary>
    [Description("darwin")]
    [Display(Name = "Mac")]
    Mac,

    /// <summary>
    /// Linux
    /// </summary>
    [Description("linux")]
    [Display(Name = "Linux")]
    Linux
}
