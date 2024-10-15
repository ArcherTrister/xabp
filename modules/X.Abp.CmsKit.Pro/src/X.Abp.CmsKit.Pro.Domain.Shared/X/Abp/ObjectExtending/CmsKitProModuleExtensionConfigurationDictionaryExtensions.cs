// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.ObjectExtending.Modularity;

namespace X.Abp.ObjectExtending;

public static class CmsKitProModuleExtensionConfigurationDictionaryExtensions
{
    public static ModuleExtensionConfigurationDictionary ConfigureCmsKitPro(
      this ModuleExtensionConfigurationDictionary modules,
      Action<CmsKitProModuleExtensionConfiguration> configureAction)
    {
        return modules.ConfigureModule(CmsKitProModuleExtensionConsts.ModuleName, configureAction);
    }
}
