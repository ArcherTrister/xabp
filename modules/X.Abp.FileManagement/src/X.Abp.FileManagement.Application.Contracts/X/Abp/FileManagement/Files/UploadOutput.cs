// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.FileManagement.Files;
public class UploadOutput
{
    public Guid FileId { get; set; }

    public string UniqueFileName { get; set; }

    public string WebUrl { get; set; }
}
