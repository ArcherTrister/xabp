// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using JetBrains.Annotations;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

using Volo.Abp.Identity;
using Volo.Abp.Identity.Localization;
using Volo.Abp.Localization;

namespace X.Abp.Identity;

[Serializable]
public class XAbpIdentityResultException : AbpIdentityResultException
{
    public XAbpIdentityResultException([NotNull] IdentityResult identityResult)
    : base(identityResult)
    {
    }

    public override string LocalizeMessage(LocalizationContext context)
    {
        var localizer = context.LocalizerFactory.Create<IdentityResource>();

        SetData(localizer);

        return IdentityResult.LocalizeIdentityErrors(localizer);
    }
}
