// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.VersionManagement.AppEditions.Dtos;

public class UpdateAppEditionDto
{
    /// <summary>
    /// 更新内容
    /// </summary>
    [StringLength(500)]
    public string UpdateContent { get; set; }

    public string ConcurrencyStamp { get; set; }
}
