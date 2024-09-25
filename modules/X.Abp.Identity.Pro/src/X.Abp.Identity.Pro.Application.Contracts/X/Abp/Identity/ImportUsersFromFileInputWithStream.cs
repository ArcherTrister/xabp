// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System.ComponentModel.DataAnnotations;

using Volo.Abp.Content;

namespace X.Abp.Identity;
public class ImportUsersFromFileInputWithStream
{
    public IRemoteStreamContent File { get; set; }

    [Range(1, 2)]
    public ImportUsersFromFileType FileType { get; set; }
}
