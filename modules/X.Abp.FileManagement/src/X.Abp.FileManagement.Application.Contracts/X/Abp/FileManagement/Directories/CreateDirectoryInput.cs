// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.ComponentModel.DataAnnotations;

using Volo.Abp.Validation;

namespace X.Abp.FileManagement.Directories;

public class CreateDirectoryInput
{
    public Guid? ParentId { get; set; }

    [Required]
    [DynamicStringLength(typeof(DirectoryDescriptorConsts), nameof(DirectoryDescriptorConsts.MaxNameLength))]
    public string Name { get; set; }
}
