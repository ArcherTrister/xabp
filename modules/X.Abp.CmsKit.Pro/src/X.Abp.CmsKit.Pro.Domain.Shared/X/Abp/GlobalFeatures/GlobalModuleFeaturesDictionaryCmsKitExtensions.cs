// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;
using System.Collections.Generic;

using Volo.Abp;
using Volo.Abp.GlobalFeatures;

using X.Abp.CmsKit.GlobalFeatures;

namespace X.Abp.GlobalFeatures;

public static class GlobalModuleFeaturesDictionaryCmsKitExtensions
{
    public static GlobalCmsKitProFeatures CmsKitPro(
      this GlobalModuleFeaturesDictionary modules)
    {
        Check.NotNull(modules, nameof(modules));
        return modules.GetOrAdd(nameof(CmsKitPro), () => new GlobalCmsKitProFeatures(modules.FeatureManager)) as GlobalCmsKitProFeatures;
    }

    public static GlobalModuleFeaturesDictionary CmsKitPro(
        this GlobalModuleFeaturesDictionary modules,
        Action<GlobalCmsKitProFeatures> configureAction)
    {
        Check.NotNull(configureAction, nameof(configureAction));
        configureAction(modules.CmsKitPro());
        return modules;
    }
}
