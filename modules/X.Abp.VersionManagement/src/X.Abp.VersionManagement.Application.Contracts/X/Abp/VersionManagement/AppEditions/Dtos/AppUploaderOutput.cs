// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.VersionManagement.AppEditions.Dtos;

public class AppUploaderOutput
{
    /// <summary>
    /// 应用程序名称
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// 文件名称
    /// </summary>
    public string UniqueFileName { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    public string FileExt { get; set; }

    /// <summary>
    /// Hash
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// MimeType
    /// </summary>
    public string MimeType { get; set; }

    /// <summary>
    /// 当前版本
    /// </summary>
    public string Version { get; set; }

    ///// <summary>
    ///// 应用程序平台
    ///// </summary>
    // public string Platform { get; set; }

    /// <summary>
    /// 平台类型
    /// </summary>
    public PlatformType PlatformType { get; set; }

    /// <summary>
    /// 发行版
    /// </summary>
    public string Channel { get; set; }

    /// <summary>
    /// 系统架构
    /// </summary>
    public string Arch { get; set; }

    ///// <summary>
    ///// 下载地址
    ///// </summary>
    // public string DownloadPath { get; set; }
}
