// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.IO;
using System.Linq;

using JetBrains.Annotations;

using Volo.Abp;

using X.Abp.FileManagement.Directories;
using X.Abp.FileManagement.Files;

namespace X.Abp.FileManagement;

public static class FileNameValidator
{
    public static bool IsValidName([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));

        // con is not valid folder/file name for Windows
        return !(Path.GetInvalidFileNameChars().Any(name.Contains) || name == "con");
    }

    public static string CheckFileName([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), FileDescriptorConsts.MaxNameLength);
        return !IsValidName(name) ? throw new InvalidFileNameException(name) : name;
    }

    public static string CheckDirectoryName([NotNull] string name)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name), DirectoryDescriptorConsts.MaxNameLength);

        return !IsValidName(name) ? throw new InvalidDirectoryNameException(name) : name;
    }
}
