// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.FileManagement.Files;

public class FileUploadPreInfoRequest
{
    public Guid? DirectoryId { get; set; }

    public string FileName { get; set; }

    public long Size { get; set; }
}
