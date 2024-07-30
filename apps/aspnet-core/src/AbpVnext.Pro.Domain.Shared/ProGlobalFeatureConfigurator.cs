// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using Volo.Abp.GlobalFeatures;
using Volo.Abp.Threading;

using X.Abp.GlobalFeatures;

namespace AbpVnext.Pro;

public static class ProGlobalFeatureConfigurator
{
    private static readonly OneTimeRunner OneTimeRunner = new();

    public static void Configure()
    {
        OneTimeRunner.Run(() =>
        {
            /* You can configure (enable/disable) global features of the used modules here.
             * Please refer to the documentation to learn more about the Global Features System:
             * https://docs.abp.io/en/abp/latest/Global-Features
             */
            GlobalFeatureManager.Instance.Modules.CmsKit(cmsKit =>
            {
                cmsKit.EnableAll();
            });
            GlobalFeatureManager.Instance.Modules.CmsKitPro(cmsKitPro =>
            {
                cmsKitPro.EnableAll();
            });
        });
    }
}
