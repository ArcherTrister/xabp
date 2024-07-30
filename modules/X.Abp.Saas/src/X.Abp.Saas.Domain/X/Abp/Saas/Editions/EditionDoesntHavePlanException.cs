// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;

namespace X.Abp.Saas.Editions;

[Serializable]
public class EditionDoesntHavePlanException : BusinessException
{
    public EditionDoesntHavePlanException(Guid editionId)
        : base(nameof(editionId))
    {
        EditionId = editionId;
        Code = SaasErrorCodes.Edition.EditionDoesntHavePlan;
        WithData(nameof(editionId), EditionId);
    }

    public virtual Guid EditionId { get; protected set; }
}
