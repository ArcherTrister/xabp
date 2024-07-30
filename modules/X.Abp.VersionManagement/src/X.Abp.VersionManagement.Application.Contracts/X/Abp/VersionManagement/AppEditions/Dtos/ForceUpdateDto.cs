// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.VersionManagement.AppEditions.Dtos;

public class ForceUpdateDto
{
    /// <summary>
    /// 是否强制更新
    /// </summary>
    public bool IsForceUpdate { get; set; }

    /// <summary>
    /// 安卓下载地址
    /// </summary>
    public string DownloadPath { get; set; }

    /// <summary>
    /// 更新内容
    /// </summary>
    public string UpdateContent { get; set; }

    /// <summary>
    /// 当前版本
    /// </summary>
    public string CurrentVersion { get; set; }
}
