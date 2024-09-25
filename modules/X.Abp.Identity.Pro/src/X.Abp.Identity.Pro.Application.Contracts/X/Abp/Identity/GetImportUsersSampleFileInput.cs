// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

namespace X.Abp.Identity;
public class GetImportUsersSampleFileInput
{
    [Range(1, 2)]
    public ImportUsersFromFileType FileType { get; set; }

    [Required]
    public string Token { get; set; }
}
