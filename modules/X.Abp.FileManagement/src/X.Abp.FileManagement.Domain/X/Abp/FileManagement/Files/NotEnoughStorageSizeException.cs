// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using JetBrains.Annotations;

using Volo.Abp;

namespace X.Abp.FileManagement.Files;

public class NotEnoughStorageSizeException : BusinessException
{
    public NotEnoughStorageSizeException([NotNull] string total, [NotNull] string remaining)
    {
        Code = FileManagementErrorCodes.NotEnoughStorageSize;
        WithData("StorageSize", total).WithData("RemainingSize", remaining);
    }
}
