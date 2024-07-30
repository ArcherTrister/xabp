// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.Application.Services;
using Volo.Abp.OpenIddict.Localization;

namespace X.Abp.OpenIddict;

public abstract class OpenIddictProAppServiceBase : ApplicationService
{
    protected OpenIddictProAppServiceBase()
    {
        LocalizationResource = typeof(AbpOpenIddictResource);
        ObjectMapperContext = typeof(AbpOpenIddictProApplicationModule);
    }

    protected virtual Guid ConvertIdentifierFromString(string identifier)
    {
        return !string.IsNullOrEmpty(identifier) ? Guid.Parse(identifier) : Guid.NewGuid();
    }

    protected virtual string ConvertIdentifierToString(Guid identifier)
    {
        return identifier.ToString("D");
    }
}
