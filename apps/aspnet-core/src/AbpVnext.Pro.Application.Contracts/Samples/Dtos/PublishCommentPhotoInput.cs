// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace AbpVnext.Pro.Samples.Dtos;
public class PublishCommentPhotoInput
{
    public string CommenterUserName { get; set; }

    public string Comment { get; set; }

    public Guid PhotoId { get; set; }

    public Guid PhotoOwnerUserId { get; set; }
}
