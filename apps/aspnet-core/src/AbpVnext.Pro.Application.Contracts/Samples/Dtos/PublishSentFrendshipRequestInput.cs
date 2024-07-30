// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace AbpVnext.Pro.Samples.Dtos;
public class PublishSentFrendshipRequestInput
{
    public string SenderUserName { get; set; }

    public string FriendshipMessage { get; set; }

    public Guid TargetUserId { get; set; }
}
