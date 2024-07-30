// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.VersionManagement.AppEditions.Dtos;

public class AppEditionDto
{
    public Guid Id { get; set; }

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

    ///// <summary>
    ///// 版本
    ///// </summary>
    // public Version Version { get; set; }

    /// <summary>
    /// 当前版本
    /// </summary>
    public string Version { get; set; }

    ///// <summary>
    ///// 应用程序平台
    ///// </summary>
    // public string Platform { get; set; }

    /// <summary>
    /// 发行版
    /// </summary>
    public string Channel { get; set; }

    /// <summary>
    /// 系统架构
    /// </summary>
    public string Arch { get; set; }

    /// <summary>
    /// 下载地址
    /// </summary>
    public string DownloadPath { get; set; }

    /// <summary>
    /// 更新内容
    /// </summary>
    public string UpdateContent { get; set; }

    /// <summary>
    /// 平台类型
    /// </summary>
    public PlatformType PlatformType { get; set; }

    /// <summary>
    /// 发布状态
    /// </summary>
    public PublishType PublishType { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreationTime { get; set; }

    public string ConcurrencyStamp { get; set; }
}
