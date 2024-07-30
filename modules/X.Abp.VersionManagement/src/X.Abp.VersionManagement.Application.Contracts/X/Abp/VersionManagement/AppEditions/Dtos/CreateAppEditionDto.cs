// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.VersionManagement.AppEditions.Dtos;

public class CreateAppEditionDto
{
    /// <summary>
    /// 应用程序名称
    /// </summary>
    [Required]
    [StringLength(20)]
    public string AppName { get; set; }

    /// <summary>
    /// 唯一文件名称
    /// </summary>
    [Required]
    [StringLength(64)]
    public string UniqueFileName { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    [Required]
    [StringLength(5)]
    public string FileExt { get; set; }

    /// <summary>
    /// Hash
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Hash { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long FileSize { get; set; }

    public string MimeType { get; set; }

    /// <summary>
    /// 当前版本
    /// </summary>
    [Required]
    [StringLength(20)]
    public string Version { get; set; }

    ///// <summary>
    ///// 应用程序平台
    ///// </summary>
    // [StringLength(20)]
    // public string Platform { get; set; }

    /// <summary>
    /// 平台类型
    /// </summary>
    public PlatformType PlatformType { get; set; }

    /// <summary>
    /// 发行版
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Channel { get; set; }

    /// <summary>
    /// 架构
    /// </summary>
    [Required]
    [StringLength(10)]
    public string Arch { get; set; }

    ///// <summary>
    ///// 下载地址
    ///// </summary>
    // [Required]
    // [StringLength(200)]
    // public string DownloadPath { get; set; }

    /// <summary>
    /// 更新内容
    /// </summary>
    [StringLength(500)]
    public string UpdateContent { get; set; }
}
