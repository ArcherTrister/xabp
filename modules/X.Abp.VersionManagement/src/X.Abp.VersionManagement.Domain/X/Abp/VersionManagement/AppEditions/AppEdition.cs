// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace X.Abp.VersionManagement.AppEditions;

public class AppEdition : CreationAuditedAggregateRoot<Guid>, IMultiTenant
{
    public virtual Guid? TenantId { get; protected set; }

    /// <summary>
    /// 应用程序名称
    /// </summary>
    public virtual string AppName { get; set; }

    /// <summary>
    /// 唯一文件名称
    /// </summary>
    public virtual string UniqueFileName { get; set; }

    /// <summary>
    /// 文件扩展名
    /// </summary>
    public virtual string FileExt { get; set; }

    /// <summary>
    /// MimeType
    /// </summary>
    public virtual string MimeType { get; set; }

    /// <summary>
    /// Hash
    /// </summary>
    public virtual string Hash { get; set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public virtual long FileSize { get; set; }

    /// <summary>
    /// 当前版本
    /// </summary>
    public virtual string Version { get; set; }

    /// <summary>
    /// 主要版本
    /// </summary>
    public virtual int Major { get; set; }

    /// <summary>
    /// 次要版本
    /// </summary>
    public virtual int Minor { get; set; }

    /// <summary>
    /// 构建版本
    /// </summary>
    public virtual int Build { get; set; }

    /// <summary>
    /// 修正版本
    /// </summary>
    public virtual int Revision { get; set; }

    /// <summary>
    /// 发布日期 2022-11-22T06:42:20.222Z
    /// </summary>
    public virtual string ReleaseDate { get; set; }

    // /// <summary>
    // /// 应用程序平台
    // /// </summary>
    // [StringLength(20)]
    // [Required]
    // public virtual string Platform { get; set; }

    /// <summary>
    /// 发行版
    /// </summary>
    public virtual string Channel { get; set; }

    /// <summary>
    /// 架构
    /// </summary>
    public virtual string Arch { get; set; }

    /// <summary>
    /// 下载地址
    /// </summary>
    public virtual string DownloadPath { get; set; }

    /// <summary>
    /// 更新内容
    /// </summary>
    public virtual string UpdateContent { get; set; }

    /// <summary>
    /// 平台类型
    /// </summary>
    public virtual PlatformType PlatformType { get; set; }

    /// <summary>
    /// 发布状态
    /// </summary>
    public virtual PublishType PublishType { get; set; }

    /// <summary>
    /// 是否需要管理员权限
    /// </summary>
    public virtual bool IsAdminRightsRequired { get; set; }
}
