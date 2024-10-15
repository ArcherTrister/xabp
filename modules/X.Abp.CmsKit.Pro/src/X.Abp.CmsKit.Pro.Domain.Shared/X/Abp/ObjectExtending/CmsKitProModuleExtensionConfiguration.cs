// Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
// See https://github.com/ArcherTrister/xabp
// for more information concerning the license and the contributors participating to this project.

using System;

using Volo.Abp.ObjectExtending.Modularity;

namespace X.Abp.ObjectExtending;

public class CmsKitProModuleExtensionConfiguration : ModuleExtensionConfiguration
{
    public CmsKitProModuleExtensionConfiguration ConfigureNewsletterRecord(
      Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(CmsKitProModuleExtensionConsts.EntityNames.NewsletterRecord, configureAction);
    }

    public CmsKitProModuleExtensionConfiguration ConfigurePoll(
      Action<EntityExtensionConfiguration> configureAction)
    {
        return this.ConfigureEntity(CmsKitProModuleExtensionConsts.EntityNames.Poll, configureAction);
    }
}
