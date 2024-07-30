// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.Collections.Generic;

namespace X.Abp.VersionManagement.AppEditions.Dtos;

public class ElectronAppInfo
{
    /// <summary>
    /// 版本
    /// </summary>
    public string Version { get; set; }

    /// <summary>
    /// Files
    /// </summary>
    public List<FilesItem> Files { get; set; } = new List<FilesItem>();

    /// <summary>
    /// Path
    /// </summary>
    public string Path { get; set; }

    /// <summary>
    /// Sha512
    /// </summary>
    public string Sha512 { get; set; }

    /// <summary>
    /// 发布日期
    /// </summary>
    public string ReleaseDate { get; set; }

    /// <summary>
    /// 更新内容
    /// </summary>
    public string UpdateContent { get; set; }

    public class FilesItem
    {
        /// <summary>
        /// Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Sha512
        /// </summary>
        public string Sha512 { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 是否申请管理员权限
        /// </summary>
        public bool IsAdminRightsRequired { get; set; }
    }
}
