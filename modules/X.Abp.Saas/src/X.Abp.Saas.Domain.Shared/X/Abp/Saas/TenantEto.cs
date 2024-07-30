// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Auditing;

namespace X.Abp.Saas;

[Serializable]
public class TenantEto : IHasEntityVersion
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public Guid? EditionId { get; set; }

    public DateTime? EditionEndDateUtc { get; set; }

    public int EntityVersion { get; set; }
}
