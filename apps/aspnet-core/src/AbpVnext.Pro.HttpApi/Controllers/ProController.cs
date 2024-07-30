// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AbpVnext.Pro.Localization;

using Volo.Abp.AspNetCore.Mvc;

namespace AbpVnext.Pro.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class ProController : AbpControllerBase
{
    protected ProController()
    {
        LocalizationResource = typeof(ProResource);
    }
}
