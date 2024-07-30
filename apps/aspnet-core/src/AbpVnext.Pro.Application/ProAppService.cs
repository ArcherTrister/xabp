// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using AbpVnext.Pro.Localization;

using Volo.Abp.Application.Services;

namespace AbpVnext.Pro;

/* Inherit your application services from this class.
 */
public abstract class ProAppService : ApplicationService
{
    protected ProAppService()
    {
        LocalizationResource = typeof(ProResource);
    }
}
