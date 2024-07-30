// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace X.Abp.Gdpr;

public class GdprRequest : AggregateRoot<Guid>, IHasCreationTime
{
    public Guid UserId { get; protected set; }

    public DateTime CreationTime { get; protected set; }

    public DateTime ReadyTime { get; protected set; }

    public ICollection<GdprInfo> Infos { get; protected set; }

    protected GdprRequest()
    {
    }

    public GdprRequest(Guid id, Guid userId, DateTime readyTime)
      : base(id)
    {
        UserId = userId;
        ReadyTime = readyTime;
        Infos = new Collection<GdprInfo>();
    }

    public void AddData(Guid gdprInfoId, string data, string provider)
    {
        Infos.Add(new GdprInfo(gdprInfoId, Id, data, provider));
    }
}
