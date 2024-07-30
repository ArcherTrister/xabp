// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Domain.Entities;

using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement.Directories;

public class DirectoryContentDto : IHasConcurrencyStamp
{
    public string Name { get; set; }

    public bool IsDirectory { get; set; }

    public Guid Id { get; set; }

    public int Size { get; set; }

    public FileIconInfo IconInfo { get; set; }

    public string ConcurrencyStamp { get; set; }
}
