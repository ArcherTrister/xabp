// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

namespace X.Abp.FileManagement.Files;

public class RawFileDto
{
    public byte[] Bytes { get; set; }

    public bool IsFileEmpty => Bytes == null || Bytes.Length == 0;

    public RawFileDto()
    {
    }

    public static RawFileDto EmptyResult()
    {
        return new RawFileDto() { Bytes = new byte[0] };
    }
}
