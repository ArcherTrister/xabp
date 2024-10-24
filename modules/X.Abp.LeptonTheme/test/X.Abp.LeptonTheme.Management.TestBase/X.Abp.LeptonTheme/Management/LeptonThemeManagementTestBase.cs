﻿using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Testing;

namespace X.Abp.LeptonTheme.Management
{
    public abstract class LeptonThemeManagementTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule> 
        where TStartupModule : IAbpModule
    {
        protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
        {
            options.UseAutofac();
        }
    }
}
