// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using JetBrains.Annotations;

using Volo.Abp;

namespace X.Abp.FileManagement.Files;

[Serializable]
public class FileAlreadyExistException : BusinessException
{
    public FileAlreadyExistException([NotNull] string fileName)
    {
        Code = FileManagementErrorCodes.FileAlreadyExist;
        WithData("FileName", fileName);
    }
}
