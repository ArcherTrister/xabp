// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.FileManagement.Files;

public static class FileDescriptorConsts
{
    public const string DefaultSorting = "Name asc";

    public static int MaxSizeLength = int.MaxValue;

    public static int MaxNameLength { get; set; } = byte.MaxValue;

    public static int MaxUniqueFileNameLength { get; } = 64;

    public static int MaxMimeTypeLength { get; set; } = 128;
}
