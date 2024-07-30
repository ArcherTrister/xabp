// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application.Dtos;

namespace X.Abp.VersionManagement.AppEditions.Dtos;

public class PagedAppEditionDto : PagedAndSortedResultRequestDto
{
    /// <summary>
    /// 应用程序名称
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 当前版本
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// 发行版
    /// </summary>
    public string Channel { get; set; }

    /// <summary>
    /// 系统架构
    /// </summary>
    public string Arch { get; set; }

    /// <summary>
    /// 平台类型
    /// </summary>
    public PlatformType? PlatformType { get; set; }

    ///// <summary>
    ///// 创建时间
    ///// </summary>
    // public DateTime CreationTime { get; set; }
}
