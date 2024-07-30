// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.Account.Dtos;

public class ProfilePictureSourceDto
{
    public ProfilePictureType Type { get; set; }

    public string Source { get; set; }

    public byte[] FileContent { get; set; }
}
