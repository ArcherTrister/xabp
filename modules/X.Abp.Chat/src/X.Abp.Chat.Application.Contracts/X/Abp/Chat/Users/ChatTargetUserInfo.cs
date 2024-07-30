// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.Chat.Users;

public class ChatTargetUserInfo
{
    public Guid UserId { get; set; }

    public string Name { get; set; }

    public string Surname { get; set; }

    public string Username { get; set; }
}
