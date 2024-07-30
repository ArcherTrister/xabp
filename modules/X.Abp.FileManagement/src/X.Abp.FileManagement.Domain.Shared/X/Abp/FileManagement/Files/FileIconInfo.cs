// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using JetBrains.Annotations;

using Volo.Abp;

namespace X.Abp.FileManagement.Files;

public class FileIconInfo
{
    public string Icon { get; set; }

    public FileIconType Type { get; set; }

    public FileIconInfo([NotNull] string icon, FileIconType type)
    {
        Icon = Check.NotNullOrWhiteSpace(icon, nameof(icon));
        Type = type;
    }
}
