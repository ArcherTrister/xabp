// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.FileManagement;

public static class FileManagementErrorCodes
{
    public const string InvalidDirectoryName = "FileManagement:0001";
    public const string InvalidFileName = "FileManagement:0002";
    public const string DirectoryAlreadyExist = "FileManagement:0003";
    public const string CantMovableToUnderChild = "FileManagement:0004";
    public const string FileAlreadyExist = "FileManagement:0005";
    public const string DirectoryNotExist = "FileManagement:0006";
    public const string NotEnoughStorageSize = "FileManagement:0007";
}
