// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Content;

namespace X.Abp.FileManagement.Files;
public class UploadInput
{
    [Required]
    public IRemoteStreamContent File { get; set; }

    /// <summary>
    /// 目录名称
    /// </summary>
    public string DirectoryName { get; set; }
}
