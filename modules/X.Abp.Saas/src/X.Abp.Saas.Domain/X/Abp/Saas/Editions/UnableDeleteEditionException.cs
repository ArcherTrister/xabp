// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp;

namespace X.Abp.Saas.Editions;

[Serializable]
public class UnableDeleteEditionException : BusinessException
{
    public UnableDeleteEditionException(string editionName)
      : base(nameof(editionName))
    {
        EditionName = editionName;
        Code = SaasErrorCodes.Edition.EditionUnableDelete;
        WithData(nameof(editionName), EditionName);
    }

    public virtual string EditionName { get; protected set; }
}
