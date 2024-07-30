// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Gdpr;

public class GdprInfo : Entity<Guid>
{
    public Guid RequestId { get; protected set; }

    public string Data { get; protected set; }

    public string Provider { get; protected set; }

    protected GdprInfo()
    {
    }

    public GdprInfo(Guid id, Guid requestId, string data, string provider)
      : base(id)
    {
        RequestId = requestId;
        Data = Check.NotNullOrWhiteSpace(data, nameof(data));
        Provider = Check.NotNullOrWhiteSpace(provider, nameof(provider), GdprInfoConsts.MaxProviderLength);
    }
}
