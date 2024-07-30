// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.AspNetCore.Mvc;
using Volo.CmsKit.Localization;

namespace X.Abp.CmsKit;

public abstract class CmsKitProController : AbpControllerBase
{
    protected CmsKitProController()
    {
        LocalizationResource = typeof(CmsKitResource);
    }
}
