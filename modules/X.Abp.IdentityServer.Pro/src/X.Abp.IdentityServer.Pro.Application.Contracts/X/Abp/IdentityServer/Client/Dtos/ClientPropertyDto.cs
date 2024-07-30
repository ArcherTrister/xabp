// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

namespace X.Abp.IdentityServer.Client.Dtos;

public class ClientPropertyDto
{
    public virtual Guid ClientId { get; set; }

    public virtual string Key { get; set; }

    public virtual string Value { get; set; }
}
