// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.FileManagement.Files;

public class FileUploadPreInfoDto
{
    public string FileName { get; set; }

    public bool DoesExist { get; set; }

    public bool HasValidName { get; set; }
}
