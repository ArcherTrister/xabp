// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Content;
using Volo.Abp.Validation;

namespace X.Abp.FileManagement.Files;

public class CreateFileInputWithStream
{
    [Required]
    [DynamicStringLength(typeof(FileDescriptorConsts), nameof(FileDescriptorConsts.MaxNameLength))]
    public string Name { get; set; }

    public IRemoteStreamContent File { get; set; }
}
