// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.Application.Services;
using Volo.Abp.IdentityServer.Localization;

namespace X.Abp.IdentityServer;

public abstract class IdentityServerAppServiceBase : ApplicationService
{
    protected IdentityServerAppServiceBase()
    {
        ObjectMapperContext = typeof(AbpIdentityServerProApplicationModule);
        LocalizationResource = typeof(AbpIdentityServerResource);
    }
}
